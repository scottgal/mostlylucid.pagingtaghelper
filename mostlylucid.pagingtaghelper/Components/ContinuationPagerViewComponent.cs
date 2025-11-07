using Microsoft.AspNetCore.Mvc;
using mostlylucid.pagingtaghelper.Models;
using mostlylucid.pagingtaghelper.Models.TagModels;

namespace mostlylucid.pagingtaghelper.Components;

/// <summary>
/// ViewComponent for rendering continuation token-based pagination.
/// </summary>
public class ContinuationPagerViewComponent : ViewComponent
{
    /// <summary>
    /// Invokes the view component with the specified model.
    /// </summary>
    /// <param name="model">The continuation pager view model.</param>
    /// <returns>The rendered view result.</returns>
    public IViewComponentResult Invoke(ContinuationPagerViewModel model)
    {
        // Determine the view to render based on ViewType and UseLocalView
        string viewName = GetViewName(model);

        return View(viewName, model);
    }

    private string GetViewName(ContinuationPagerViewModel model)
    {
        if (model.UseLocalView)
        {
            return "Custom";
        }

        return model.ViewType switch
        {
            ViewType.TailwindAndDaisy => "Default",
            ViewType.Tailwind => "TailwindView",
            ViewType.Bootstrap => "BootstrapView",
            ViewType.Plain => "PlainView",
            ViewType.NoJS => "NoJSView",
            _ => "Default"
        };
    }
}
