
using Microsoft.AspNetCore.Mvc;
using mostlylucid.pagingtaghelper.Models;
using mostlylucid.pagingtaghelper.Models.TagModels;

namespace mostlylucid.pagingtaghelper.Components;


public class PageSizeViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PageSizeModel model)
        {
            if(model.Model != null)
            {
                model.LinkUrl ??= model.Model.LinkUrl;
                
                if(model.Model is IPagingSearchModel searchModel)
                {
                    model.SearchTerm ??= searchModel.SearchTerm;
                }
            }
            
            var viewName = "Components/Pager/Default";

            var useLocalView = model.UseLocalView;

            return (useLocalView, model.ViewType) switch
            {
                (true, ViewType.Custom) when ViewExists(viewName) => View(viewName, model),
                (true, ViewType.Custom) when !ViewExists(viewName) => throw new ArgumentException("View not found: " + viewName),
                (false, ViewType.Bootstrap) => View("/Areas/Components/Views/PageSize/BootstrapView.cshtml", model),
                (false, ViewType.Plain) => View("/Areas/Components/Views/PageSize/PlainView.cshtml", model),
                (false, ViewType.TailwindAndDaisy) => View("/Areas/Components/Views/PageSize/Default.cshtml", model),
                _ => View("/Areas/Components/Views/PageSize/Default.cshtml", model)
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