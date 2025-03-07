
using Microsoft.AspNetCore.Mvc;
using mostlylucid.pagingtaghelper.Models;

namespace mostlylucid.pagingtaghelper.Components;


public class PagerViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PagerViewModel model)
        {
            if(model.Model != null)
            {
                model.Page ??= model.Model.Page;
                model.PageSize ??= model.Model.PageSize;
                model.TotalItems ??= model.Model.TotalItems;
                model.LinkUrl ??= model.Model.LinkUrl;
                if(model.Model is IPagingSearchModel searchModel)
                {
                    model.SearchTerm ??= searchModel.SearchTerm;
                }
            }
            
            if(model.Page == null || model.PageSize == null || model.TotalItems == null || model.LinkUrl == null)
            {
                throw new ArgumentException("Page, PageSize, TotalItems, and LinkUrl required.");
            }
            // Calculate total pages based on total items and page size.
            model.TotalPages = (int)Math.Ceiling((double)model.TotalItems.Value / model.PageSize.Value);

            // Ensure the current page is within valid bounds.
            model.Page = Math.Max(1, Math.Min(model.Page.Value, model.TotalPages));

            return View("Default", model);

        }
    }

    public class PagerViewModel
    {
        public IPagingModel? Model { get; set; }
        
        public string? SearchTerm { get; set; }
        // Required properties
        public string? LinkUrl { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int? TotalItems { get; set; }

        // Optional properties with defaults (using DaisyUI/Tailwind classes)
        public int PagesToDisplay { get; set; } = 5;
        public string CssClass { get; set; } = "btn-group";  // DaisyUI grouping style
        public string FirstPageText { get; set; } = "«";
        public string PreviousPageText { get; set; } = "‹ Previous";
        public string SkipBackText { get; set; } = "..";
        public string SkipForwardText { get; set; } = "..";
        public string NextPageText { get; set; } = "Next ›";
        public string NextPageAriaLabel { get; set; } = "go to next page";
        public string LastPageText { get; set; } = "»";
        public bool FirstLastNavigation { get; set; } = true;
        public bool SkipForwardBackNavigation { get; set; } = true;

        // Calculated value
        public int TotalPages { get; set; }

        // Optional htmx integration:
        // If set (e.g. "#content"), pagination links will include htmx attributes.
        public string HtmxTarget { get; set; }
    }