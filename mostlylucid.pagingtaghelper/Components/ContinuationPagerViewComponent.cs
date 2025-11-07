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
            return "Components/ContinuationPager/Default";
        }

        return model.ViewType switch
        {
            ViewType.TailwindAndDaisy => "/Areas/Components/Views/ContinuationPager/Default.cshtml",
            ViewType.Tailwind => "/Areas/Components/Views/ContinuationPager/TailwindView.cshtml",
            ViewType.Bootstrap => "/Areas/Components/Views/ContinuationPager/BootstrapView.cshtml",
            ViewType.Plain => "/Areas/Components/Views/ContinuationPager/PlainView.cshtml",
            ViewType.NoJS => "/Areas/Components/Views/ContinuationPager/NoJSView.cshtml",
            _ => "/Areas/Components/Views/ContinuationPager/Default.cshtml"
        };
    }
}
