
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

            var viewName = "Components/Pager/Default";

            var useLocalView = model.UseLocalView;

            return useLocalView switch
            {
                true when ViewExists(viewName) => View(viewName, model),
                true when !ViewExists(viewName) => throw new ArgumentException("View not found: " + viewName),
                _ => View("/Areas/Components/Views/Pager/Default.cshtml", model)
            };

            // If the view exists in the app, use it; otherwise, use the fallback RCL view
        }
        /// <summary>
        /// Checks if a view exists in the consuming application.
        /// </summary>
        private bool ViewExists(string viewName)
        {
            var result = ViewEngine.FindView(ViewContext, viewName, false);
            return result.Success;
        }
    }