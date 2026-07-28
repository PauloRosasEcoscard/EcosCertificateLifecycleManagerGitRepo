using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using System.Threading.Tasks;

namespace EcosCLM.Web.Infrastructure.TagHelpers
{
    [HtmlTargetElement("ecos-form")]
    public class EcosFormTagHelper : TagHelper
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EcosFormTagHelper(IAntiforgery antiforgery, IHttpContextAccessor httpContextAccessor)
        {
            _antiforgery = antiforgery;
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("title")]
        public string Title { get; set; } = string.Empty;

        [HtmlAttributeName("subtitle")]
        public string Subtitle { get; set; } = string.Empty;

        [HtmlAttributeName("handler")]
        public string? Handler { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var antiforgeryToken = _antiforgery.GetAndStoreTokens(httpContext!);
            var currentPath = httpContext?.Request.Path.Value ?? string.Empty;

            // Transforma a tag do TagHelper no próprio <form>
            var formAction = string.IsNullOrWhiteSpace(Handler)
                                ? currentPath
                                : $"{currentPath}?handler={Uri.EscapeDataString(Handler)}";

            output.TagName = "form";
            output.Attributes.SetAttribute("method", "post");
            output.Attributes.SetAttribute("action", formAction);
            output.Attributes.SetAttribute("class", "ecos-card p-0");

            var headerHtml = new StringBuilder();

            // 1. Token Anti-Forgery + Cabeçalho
            headerHtml.Append($"<input type='hidden' name='__RequestVerificationToken' value='{antiforgeryToken.RequestToken}' />");
            headerHtml.Append("<div class='p-4 border-bottom border-color-subtle'>");
            headerHtml.Append($"<h4 class='fw-bold mb-0 text-main'>{Title}</h4>");

            if (!string.IsNullOrEmpty(Subtitle))
            {
                headerHtml.Append($"<p class='text-muted small mb-0 mt-1'>{Subtitle}</p>");
            }

            headerHtml.Append("</div>");
            headerHtml.Append("<div class='p-4'>");

            output.PreContent.SetHtmlContent(headerHtml.ToString());

            // 2. Fechamento do corpo + Rodapé de Ações
            var footerHtml = new StringBuilder();
            footerHtml.Append("</div>"); // Fecha o div.p-4 dos campos
            footerHtml.Append(@"
                <div class='p-4 bg-light bg-opacity-10 border-top border-color-subtle d-flex justify-content-end gap-2 rounded-bottom-3'>
                    <a href='./Index' class='btn btn-soft-danger px-4 fw-semibold' style='min-width: 120px;'>
                        Cancel
                    </a>
                    <button type='submit' class='btn btn-ecos-action px-4 fw-semibold' style='min-width: 120px;'>
                        Save Changes
                    </button>
                </div>");

            output.PostContent.SetHtmlContent(footerHtml.ToString());
        }
    }
}