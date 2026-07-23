using JW;
using Microsoft.AspNetCore.Mvc;

namespace EcosCLM.Web.ViewComponents
{
    public class PaginatorViewComponent : ViewComponent
    {
        public PaginatorViewComponent()
        { }

        public async Task<IViewComponentResult> InvokeAsync(Pager model, string url)
        {
            await Task.Run(() => { }).ConfigureAwait(false);

            return View(new Paginatorbar
            {
                Pager = model,
                Url = string.IsNullOrEmpty(url) ? "Index" : url
            });
        }
    }

    public class Paginatorbar
    {
        public string Url { get; set; } = "Index";
        public Pager? Pager { get; set; }
    }
}
