using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using mostlylucid.pagingtaghelper.Models;

namespace mostlylucid.pagingtaghelper.Components;

/// <summary>
/// A tag helper that renders a pagination control using a view component.
/// </summary>
[HtmlTargetElement("paging", TagStructure = TagStructure.NormalOrSelfClosing)]
public class PagerTagHelper : TagHelper
{
    

    /// <summary>
    /// The paging model containing the pagination details (OPTIONAL).
    /// </summary>
    [HtmlAttributeName("model")]
    public IPagingModel? Model { get; set; }
    
    [HtmlAttributeName("view-type")]
    public ViewType ViewType { get; set; } = Models.ViewType.TailwindANdDaisy;

    /// <summary>
    /// Whether to enable HTMX use for the pagesize component. Defaults to true.
    /// </summary>
    [HtmlAttributeName("use-htmx")] public bool UseHtmx { get; set; } = true;
    /// <summary>
    /// Optionally pass in an id for the pager component.
    /// </summary>
    [HtmlAttributeName("id")]
    public string? PagerId { get; set; }

    /// <summary>
    /// The view model for the pager component (OPTIONAL).
    /// </summary>
    [HtmlAttributeName("pagingmodel")]
    public PagerViewModel? PagerModel { get; set; } = null;

    /// <summary>
    /// Determines whether the page size selection is shown.
    /// </summary>
    [HtmlAttributeName("show-pagesize")]
    public bool ShowPageSize { get; set; } = true;
    
    /// <summary>
    /// The search term used in pagination filtering.
    /// </summary>
    [HtmlAttributeName("search-term")]
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// The base URL for pagination links.
    /// </summary>
    [HtmlAttributeName("link-url")]
    public string? LinkUrl { get; set; }

    /// <summary>
    /// The current page number.
    /// </summary>
    [HtmlAttributeName("page")]
    public int? Page { get; set; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    [HtmlAttributeName("page-size")]
    public int? PageSize { get; set; }
    
    /// <summary>
    /// Sets the view to use the local view or to use the taghelper view.
    /// </summary>

    [HtmlAttributeName("use-local-view")]
    public bool UseLocalView { get; set; } = false;
    
    /// <summary>
    /// The total number of items in the pagination set.
    /// </summary>
    [HtmlAttributeName("total-items")]
    public int? TotalItems { get; set; }

    /// <summary>
    /// The number of pages to display in the pager navigation.
    /// </summary>
    [HtmlAttributeName("pages-to-display")]
    public int PagesToDisplay { get; set; } = 5;

    /// <summary>
    /// The CSS class applied to the pager container.
    /// </summary>
    [HtmlAttributeName("css-class")]
    public string CssClass { get; set; } = "btn-group";

    /// <summary>
    /// Text for the first page navigation link.
    /// </summary>
    [HtmlAttributeName("first-page-text")]
    public string FirstPageText { get; set; } = "«";

    /// <summary>
    /// Text for the previous page navigation link.
    /// </summary>
    [HtmlAttributeName("previous-page-text")]
    public string PreviousPageText { get; set; } = "‹ Previous";

    /// <summary>
    /// Text for skipping backward in pagination.
    /// </summary>
    [HtmlAttributeName("skip-back-text")]
    public string SkipBackText { get; set; } = "..";

    /// <summary>
    /// Text for skipping forward in pagination.
    /// </summary>
    [HtmlAttributeName("skip-forward-text")]
    public string SkipForwardText { get; set; } = "..";

    /// <summary>
    /// Text for the next page navigation link.
    /// </summary>
    [HtmlAttributeName("next-page-text")]
    public string NextPageText { get; set; } = "Next ›";

    /// <summary>
    /// ARIA label for the next page navigation link.
    /// </summary>
    [HtmlAttributeName("next-page-aria-label")]
    public string NextPageAriaLabel { get; set; } = "go to next page";

    /// <summary>
    /// Text for the last page navigation link.
    /// </summary>
    [HtmlAttributeName("last-page-text")]
    public string LastPageText { get; set; } = "»";

    /// <summary>
    /// Indicates whether first and last page navigation links should be displayed.
    /// </summary>
    [HtmlAttributeName("first-last-navigation")]
    public bool FirstLastNavigation { get; set; } = true;

    /// <summary>
    /// Indicates whether skip forward/backward navigation should be enabled.
    /// </summary>
    [HtmlAttributeName("skip-forward-back-navigation")]
    public bool SkipForwardBackNavigation { get; set; } = true;

    /// <summary>
    /// Specifies the HTMX target for AJAX-based pagination.
    /// </summary>
    [HtmlAttributeName("htmx-target")]
    public string HtmxTarget { get; set; } = "";

    /// <summary>
    /// The current view context, automatically injected.
    /// </summary>
    [ViewContext]
    [HtmlAttributeNotBound]
    public required ViewContext ViewContext { get; set; }

    /// <summary>
    /// Processes the tag helper to generate the pagination component.
    /// </summary>
    /// <param name="context">The tag helper context.</param>
    /// <param name="output">The tag helper output.</param>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
   
        output.TagName = "div";
        
        //Remove all the properties that are not needed for the rendered content.
        output.Attributes.RemoveAll("page");
        output.Attributes.RemoveAll("link-url");
        output.Attributes.RemoveAll("page-size");
        output.Attributes.RemoveAll("total-items");
        output.Attributes.RemoveAll("pages-to-display");
        output.Attributes.RemoveAll("css-class");
        output.Attributes.RemoveAll("first-page-text");
        output.Attributes.RemoveAll("previous-page-text");
        output.Attributes.RemoveAll("skip-back-text");
        output.Attributes.RemoveAll("skip-forward-text");
        output.Attributes.RemoveAll("next-page-text");
        output.Attributes.RemoveAll("next-page-aria-label");
        output.Attributes.RemoveAll("last-page-text");
        output.Attributes.RemoveAll("first-last-navigation");
        output.Attributes.RemoveAll("skip-forward-back-navigation");
        output.Attributes.RemoveAll("model");
        output.Attributes.RemoveAll("show-pagesize");
        output.Attributes.RemoveAll("pagingmodel");
        output.Attributes.RemoveAll("use-local-view");
        
        var pagerId =  PagerId ?? $"pager-{Guid.NewGuid():N}";
        var linkUrl = LinkUrl ?? ViewContext.HttpContext.Request.Path;
        PageSize = Model?.PageSize ?? PageSize ?? 10;
        Page = Model?.Page ?? Page ?? 1;
        ViewType = Model?.ViewType ?? ViewType;
        TotalItems = Model?.TotalItems ?? TotalItems ?? 0;
        if(Model is IPagingSearchModel searchModel)
            SearchTerm = searchModel?.SearchTerm ?? SearchTerm ?? "";
        output.Attributes.SetAttribute("id", pagerId);
        var viewComponentHelper = (IViewComponentHelper)ViewContext.HttpContext.RequestServices.GetService(typeof(IViewComponentHelper))!;
        ((IViewContextAware)viewComponentHelper).Contextualize(ViewContext);

        var pagerModel = PagerModel ?? new PagerViewModel()
        {
            
            ViewType = ViewType,
            UseLocalView = UseLocalView,
            UseHtmx = UseHtmx,
            PagerId = pagerId,
            SearchTerm = SearchTerm,
            ShowPageSize = ShowPageSize,
            Model = Model,
            LinkUrl = linkUrl,
            Page = Page,
            PageSize = PageSize,
            TotalItems = TotalItems,
            PagesToDisplay = PagesToDisplay,
            CssClass = CssClass,
            FirstPageText = FirstPageText,
            PreviousPageText = PreviousPageText,
            SkipBackText = SkipBackText,
            SkipForwardText = SkipForwardText,
            NextPageText = NextPageText,
            NextPageAriaLabel = NextPageAriaLabel,
            LastPageText = LastPageText,
            FirstLastNavigation = FirstLastNavigation,
            SkipForwardBackNavigation = SkipForwardBackNavigation,
            HtmxTarget = HtmxTarget,
            
        };

        var result = await viewComponentHelper.InvokeAsync("Pager", pagerModel);
        output.Content.SetHtmlContent(result);
    }
}