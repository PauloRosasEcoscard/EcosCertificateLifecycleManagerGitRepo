using EcosCLM.Web.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using System.Threading.Tasks;

namespace EcosCLM.Web.Infrastructure.TagHelpers
{
    [HtmlTargetElement("ecos-grid")]
    public class EcosGridTagHelper : TagHelper
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EcosGridTagHelper(IAntiforgery antiforgery, IHttpContextAccessor httpContextAccessor)
        {
            _antiforgery = antiforgery;
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("config")]
        public GridConfiguration Config { get; set; } = null!;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Config == null) return;

            var httpContext = _httpContextAccessor.HttpContext;
            var antiforgeryToken = _antiforgery.GetAndStoreTokens(httpContext);

            var currentPath = httpContext.Request.Path.Value ?? string.Empty;

            var basePath = currentPath;
            if (basePath.EndsWith("/Index", System.StringComparison.OrdinalIgnoreCase))
            {
                basePath = basePath.Substring(0, basePath.Length - 6);
            }
            if (!basePath.EndsWith("/"))
            {
                basePath += "/";
            }

            output.TagName = "div";
            output.Attributes.SetAttribute("class", "ecos-grid-wrapper");

            var childContent = await output.GetChildContentAsync();
            var completeContent = childContent.GetContent();

            var searchAppendHtml = string.Empty;
            var cardsHtml = completeContent;

            if (completeContent.Contains("<search-append>") && completeContent.Contains("</search-append>"))
            {
                int start = completeContent.IndexOf("<search-append>") + "<search-append>".Length;
                int end = completeContent.IndexOf("</search-append>");
                if (end > start)
                {
                    searchAppendHtml = completeContent.Substring(start, end - start);
                    cardsHtml = completeContent.Substring(0, completeContent.IndexOf("<search-append>")) + completeContent.Substring(end + "</search-append>".Length);
                }
            }

            var htmlContent = new StringBuilder();

            htmlContent.Append($@"
                <div class='ecos-card p-4 mb-3'>
                    <div class='d-flex justify-content-between align-items-center pb-3 border-bottom border-color-subtle mb-3'>
                        <h4 class='fw-bold mb-0 text-main'>{Config.Title}</h4>");

            if (Config.ShowAddButton && !string.IsNullOrEmpty(Config.AddPageUrl))
            {
                var resolvedAddUrl = Config.AddPageUrl.StartsWith("/") ? Config.AddPageUrl : $"{basePath}{Config.AddPageUrl}";

                htmlContent.Append($@"
            <a href='{resolvedAddUrl}' class='btn btn-ecos-action d-flex align-items-center gap-1 px-3 py-1.5 fw-semibold text-decoration-none'>
                <i class='material-symbols-outlined fs-5'>add</i> New
            </a>");
            }

            htmlContent.Append($@"
                    </div>
                    <div class='row'>
                        <div class='col-12'>
                            <form method='post' role='form' class='m-0'>
                                <input type='hidden' name='__RequestVerificationToken' value='{antiforgeryToken.RequestToken}' />
                                
                                <div class='d-flex flex-column flex-md-row gap-2 align-items-md-center'>");

            if (!string.IsNullOrEmpty(Config.SearchPlaceholder))
            {
                htmlContent.Append($@"
                                    <div class='input-group input-group-sm max-width-300'>
                                        <span class='input-group-text bg-transparent text-muted border-end-0'>
                                            <i class='material-symbols-outlined fs-6'>search</i>
                                        </span>
                                        <input type='text' name='Search.TxName' value='{Config.SearchQuery}' class='form-control border-start-0 ps-0 shadow-none text-main' placeholder='{Config.SearchPlaceholder}' />
                                    </div>");
            }

            htmlContent.Append($" {searchAppendHtml} ");

            // Lógica Condicional: Omite os botões da Tag Helper se a View já os declarar manualmente
            if (!searchAppendHtml.Contains("handler=Search") && !searchAppendHtml.Contains("formaction"))
            {
                htmlContent.Append($@"
                                    <div class='d-flex gap-2 align-items-center flex-wrap'>
                                        <button type='submit' formaction='{currentPath}?handler=Search' class='btn btn-primary btn-sm d-flex align-items-center justify-content-center px-3' title='Pesquisar'>
                                            <i class='material-symbols-outlined fs-6'>search</i>
                                        </button>
                                        <button type='submit' formaction='{currentPath}?handler=Clear' class='btn btn-outline-secondary btn-sm d-flex align-items-center justify-content-center px-3' title='Limpar filtro'>
                                            <i class='material-symbols-outlined fs-6'>clear</i>
                                        </button>
                                    </div>");
            }

            htmlContent.Append($@"
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
                
                <div class='d-flex flex-column' style='gap: 0.5rem;'>");

            output.PreContent.SetHtmlContent(htmlContent.ToString());
            output.Content.SetHtmlContent(cardsHtml);

            var footerHtml = new StringBuilder();
            footerHtml.Append("</div>");

            if (Config.TotalPages >= 1)
            {
                footerHtml.Append($@"
                    <div class='d-flex justify-content-between align-items-center mt-4 pt-3 border-top border-color-subtle opacity-75 px-1'>
                        <div class='small text-muted'>Page <strong>{Config.CurrentPage}</strong> of <strong>{Config.TotalPages}</strong></div>
                        <nav aria-label='Page navigation'>
                            <ul class='pagination pagination-sm mb-0 gap-1'>
                                <li class='page-item {(Config.CurrentPage <= 1 ? "disabled" : "")}'>
                                    <a class='page-link rounded d-flex align-items-center justify-content-center shadow-none' href='{currentPath}?p={(Config.CurrentPage - 1)}'>
                                        <i class='material-symbols-outlined fs-6'>chevron_left</i>
                                    </a>
                                </li>");

                for (int i = 1; i <= Config.TotalPages; i++)
                {
                    footerHtml.Append($@"
                        <li class='page-item {(Config.CurrentPage == i ? "active" : "")}'>
                            <a class='page-link rounded shadow-none' href='{currentPath}?p={i}'>{i}</a>
                        </li>");
                }

                footerHtml.Append($@"
                                <li class='page-item {(Config.CurrentPage >= Config.TotalPages ? "disabled" : "")}'>
                                    <a class='page-link rounded d-flex align-items-center justify-content-center shadow-none' href='{currentPath}?p={(Config.CurrentPage + 1)}'>
                                        <i class='material-symbols-outlined fs-6'>chevron_right</i>
                                    </a>
                                </li>
                            </ul>
                        </nav>
                    </div>");
            }

            output.PostContent.SetHtmlContent(footerHtml.ToString());
        }
    }
}