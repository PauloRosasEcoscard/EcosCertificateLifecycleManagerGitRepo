using EcosCLM.Web.EcosLoginIntegration.Extensions;
using EcosCLM.Web.EcosLoginIntegration.Interfaces;
using EcosCLM.Web.EcosLoginIntegration.Model;
using JW;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EcosCLM.Web.Infrastructure.Core
{
    public class BasePageModel<T> : PageModel where T : class, new()
    {
        #region Constants
        private const string SessionFilter = "Filter";
        private const string SessionOrderBy = "OrderBy";
        private const string SessionOrderDirection = "OrderDirection";
        #endregion

        #region Parameters
        public readonly IConfiguration _config;
        private readonly IEcosLoginService _EcosLoginService;

        public Pager Pager { get; set; }
        public SelectList TotalItemsList { get; set; }
        public SelectList PageSizeList { get; set; }
        public SelectList MaxPagesList { get; set; }

        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public int MaxPages { get; set; }
        public int PageCurrent { get; set; }

        public string Filter { get; set; }
        public string OrderBy { get; set; }
        public string OrderDirection { get; set; }

        public string Email { get; set; }
        public Guid CustumerId { get; set; }

        protected List<ContextRolesViewModel> ContextRoles { get; set; } = new();

        private IHttpContextAccessor? _httpContextAccessor;
        protected IHttpContextAccessor HttpContextAccessor =>
            _httpContextAccessor ??= HttpContext?.RequestServices.GetRequiredService<IHttpContextAccessor>();
        #endregion

        #region Parameters Bindable
        [BindProperty(Name = "Item")]
        public T Item { get; set; } = new();

        public IEnumerable<T> Itens { get; set; } = [];
        #endregion

        public BasePageModel(IEcosLoginService EcosLoginService, IConfiguration config)
        {
            _EcosLoginService = EcosLoginService;
            _config = config;

            TotalItemsList = new SelectList(new[] { 10, 150, 500, 1000, 5000, 10000, 50000, 100000, 1000000 });
            PageSizeList = new SelectList(new[] { 1, 5, 10, 20, 50, 100, 200, 500, 1000 });
            MaxPagesList = new SelectList(new[] { 1, 5, 10, 20, 50, 100, 200, 500 });
        }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            TotalItems = HttpContext.Session.GetInt32("TotalItems") ?? 150;
            PageSize = HttpContext.Session.GetInt32("PageSize") ?? 10;
            MaxPages = HttpContext.Session.GetInt32("MaxPages") ?? 10;

            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                BasePageModel<T>.RedirectToLogin(context);
                return;
            }

            try
            {
                Email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
                var result = await _EcosLoginService.GetUserCustomerId(Email).ConfigureAwait(false);
                CustumerId = result.Data ?? Guid.Empty;
                var userSetProfile = User.FindFirst("Profile")?.Value;
                var isAdmin = ((int)TypeProfile.Admin).ToString();
                var isAudit = ((int)TypeProfile.Audit).ToString();

                if (userSetProfile == isAdmin)
                {
                    await next().ConfigureAwait(false); // Executa a página normalmente
                    return;
                }

                var claimsRoles = User.FindFirst(ClaimTypes.Role)?.Value;
                if (!string.IsNullOrEmpty(claimsRoles))
                {
                    var resultClaims = await _EcosLoginService.DecryptRoles<List<ContextRolesViewModel>>(claimsRoles, _config.GetEncryptKeyFromConfig()).ConfigureAwait(false);
                    ContextRoles = resultClaims.Data ?? new();
                }

                bool canViewAudit = userSetProfile == isAudit;
                string method = context.HttpContext.Request.Method.ToUpper();
                string pagePath = context.ActionDescriptor.DisplayName?.ToLower() ?? "";

                if (method == "GET")
                {
                    if (!canViewAudit && !IsValidPageMethod(pagePath, 1))
                    {
                        BasePageModel<T>.RedirectToAccessDenied(context);
                        return;
                    }
                }
                else if (method == "POST")
                {
                    bool isValidPage = true;

                    if (pagePath.EndsWith("/add"))
                        isValidPage = IsValidPageMethod(pagePath, 2);
                    else if (pagePath.EndsWith("/update"))
                        isValidPage = IsValidPageMethod(pagePath, 3);
                    else if (pagePath.EndsWith("/delete"))
                        isValidPage = IsValidPageMethod(pagePath, 4);

                    if (!isValidPage)
                    {
                        BasePageModel<T>.RedirectToAccessDenied(context);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Auth Error: {ex.Message}");
                BasePageModel<T>.RedirectToLogin(context);
                return;
            }

            await next().ConfigureAwait(false);
        }

        private bool IsValidPageMethod(string pagePath, int permissionLevel)
        {
            if (ContextRoles == null || !ContextRoles.Any()) return false;

            var pms = ContextRoles.Where(x => !string.IsNullOrEmpty(x.ModuleController) && pagePath.Contains(x.ModuleController.ToLower()));

            return permissionLevel switch
            {
                1 => pms.Any(x => x.IsView == true),
                2 => pms.Any(x => x.IsView == true && x.IsAdd == true),
                3 => pms.Any(x => x.IsView == true && x.IsUpd == true),
                4 => pms.Any(x => x.IsView == true && x.IsDel == true),
                _ => false
            };
        }

        public T GetFilters()
        {
            OrderBy = HttpContext.Session.GetString(SessionOrderBy) ?? string.Empty;
            OrderDirection = HttpContext.Session.GetString(SessionOrderDirection) ?? string.Empty;
            Filter = HttpContext.Session.GetString(SessionFilter) ?? string.Empty;

            if (!string.IsNullOrEmpty(Filter))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(Filter);
                }
                catch
                {
                    return new T();
                }
            }
            return new T();
        }

        public IActionResult OnPostSearch([FromForm] T Search, [FromQuery] string orderby = "")
        {
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            HttpContext.Session.SetString(SessionFilter, JsonConvert.SerializeObject(Search, Formatting.None, jsonSettings));

            string previousOrder = HttpContext.Session.GetString(SessionOrderBy) ?? "";

            if (!string.IsNullOrEmpty(orderby))
            {
                HttpContext.Session.SetString(SessionOrderBy, orderby);

                OrderDirection = (previousOrder == orderby && (HttpContext.Session.GetString(SessionOrderDirection) == "asc"))
                    ? "desc"
                    : "asc";

                HttpContext.Session.SetString(SessionOrderDirection, OrderDirection);
            }

            return RedirectToPage("Index");
        }

        public IActionResult OnPostClear(bool IsButton = true)
        {
            string referer = Request.Headers["Referer"].ToString();
            bool shouldClear = IsButton;

            if (!string.IsNullOrEmpty(referer) && !IsButton)
            {
                try
                {
                    var refererPath = new Uri(referer).AbsolutePath.ToLower();
                    var currentPagePath = new Uri(Request.GetDisplayUrl()).AbsolutePath.ToLower();

                    if (!refererPath.Contains(currentPagePath))
                    {
                        shouldClear = true;
                    }
                }
                catch
                {
                    shouldClear = true;
                }
            }

            if (shouldClear)
            {
                HttpContext.Session.Remove(SessionFilter);
                HttpContext.Session.Remove(SessionOrderBy);
                HttpContext.Session.Remove(SessionOrderDirection);
            }

            return RedirectToPage("Index");
        }

        #region Helpers de Redirecionamento
        private static void RedirectToLogin(PageHandlerExecutingContext context)
        {
            context.Result = new RedirectToPageResult("/authentication/login");
        }

        private static void RedirectToAccessDenied(PageHandlerExecutingContext context)
        {
            context.Result = new RedirectToPageResult("/authentication/accessdenied");
        }
        #endregion

    }
}