using System.Collections.Generic;

namespace EcosCLM.Web.Models
{
    public class GridConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string AddPageUrl { get; set; } = string.Empty;
        public string SearchPlaceholder { get; set; } = "Search...";
        public string SearchQuery { get; set; } = string.Empty;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public List<string> Headers { get; set; } = new();
        public bool ShowAddButton { get; set; } = true;
    }
}