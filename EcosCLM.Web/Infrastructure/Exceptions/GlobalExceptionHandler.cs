using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EcosCLM.Web.Infrastructure.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly ITempDataDictionaryFactory _tempDataFactory;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, ITempDataDictionaryFactory tempDataFactory)
    {
        _logger = logger;
        _tempDataFactory = tempDataFactory;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        string errorTraceId = $"ERR-{DateTime.UtcNow:yyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        _logger.LogError(exception, "Unhandled exception intercepted. Correlation ID: {ErrorTraceId}", errorTraceId);

        var statusCode = exception switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        string? acceptHeader = httpContext.Request.Headers.Accept;
        if (acceptHeader != null && acceptHeader.Contains("application/json"))
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode == StatusCodes.Status401Unauthorized ? "Unauthorized Access" : "Internal System Error",
                Detail = "An unexpected error occurred while processing your request.",
                Instance = httpContext.Request.Path
            };
            problemDetails.Extensions.Add("errorCode", errorTraceId);

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        try
        {
            // 1. Verifique se o recurso de sessão está presente antes de tentar usar o TempData
            var sessionFeature = httpContext.Features.Get<Microsoft.AspNetCore.Http.Features.ISessionFeature>();

            if (sessionFeature != null)
            {
                var tempData = _tempDataFactory.GetTempData(httpContext);

                string friendlyMessage = statusCode == StatusCodes.Status401Unauthorized
                    ? "Insufficient permissions."
                    : "Server instability.";

                tempData["modal"] = $"<div class='text-center'><p>{friendlyMessage}</p><p class='mb-0 small text-muted font-monospace'>Protocol: <strong>{errorTraceId}</strong></p></div>";

                if (httpContext.Request.Path != "/")
                {
                    httpContext.Response.Redirect("/");
                    return true;
                }
            }
            else
            {
                // 2. Se a sessão não existir (fallback), renderize uma resposta simples de erro
                // Isso evita que a aplicação quebre tentando acessar algo que não existe.
                httpContext.Response.StatusCode = statusCode;
                httpContext.Response.ContentType = "text/html; charset=utf-8";
                await httpContext.Response.WriteAsync($"<h1>Error</h1><p>Protocol: {errorTraceId}</p>");
                return true;
            }
        }
        catch (Exception fallbackEx)
        {
            _logger.LogCritical(fallbackEx, "Failed to inject TempData. Halting pipeline to prevent redirect loops. TraceId: {ErrorTraceId}", errorTraceId);
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "text/html";
        await httpContext.Response.WriteAsync($"<html><body style='font-family:sans-serif; text-align:center; padding:50px;'><h2>Critical System Failure</h2><p>Please contact support.</p><p>Protocol: <strong>{errorTraceId}</strong></p></body></html>", cancellationToken);

        return true;
    }
}