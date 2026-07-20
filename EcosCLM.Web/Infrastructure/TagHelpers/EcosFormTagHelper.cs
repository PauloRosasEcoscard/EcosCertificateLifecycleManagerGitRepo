using EcosCLM.Web.Models;
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
        public string Handler { get; set; } = "Save";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var antiforgeryToken = _antiforgery.GetAndStoreTokens(httpContext);
            var currentPath = httpContext.Request.Path.Value ?? string.Empty;

            output.TagName = "div";
            output.Attributes.SetAttribute("class", "ecos-card p-0");

            var childContent = await output.GetChildContentAsync().ConfigureAwait(false);

            var htmlContent = new StringBuilder();

            // 1. Cabeçalho do Formulário
            htmlContent.Append($@"
                <div class='p-4 border-bottom border-color-subtle'>
                    <h4 class='fw-bold mb-0 text-main'>{Title}</h4>");

            if (!string.IsNullOrEmpty(Subtitle))
            {
                htmlContent.Append($"<p class='text-muted small mb-0 mt-1'>{Subtitle}</p>");
            }

            htmlContent.Append($@"
                </div>
                <form method='post' formaction='{currentPath}?handler={Handler}'>
                    <input type='hidden' name='__RequestVerificationToken' value='{antiforgeryToken.RequestToken}' />
                    <div class='p-4'>");

            output.PreContent.SetHtmlContent(htmlContent.ToString());

            // 2. Rodapé Unificado de Ações
            var footerHtml = new StringBuilder();
            footerHtml.Append($@"
                    </div>
                    <div class='p-4 bg-light bg-opacity-10 border-top border-color-subtle d-flex justify-content-end gap-2 rounded-bottom-3'>
                        <a href='./Index' class='btn btn-soft-danger px-4 fw-semibold' style='min-width: 120px;'>
                            Cancel
                        </a>
                        <button type='submit' class='btn btn-ecos-action px-4 fw-semibold' style='min-width: 120px;'>
                            Save Changes
                        </button>
                    </div>
                </form>");

            output.PostContent.SetHtmlContent(footerHtml.ToString());
        }
    }
}