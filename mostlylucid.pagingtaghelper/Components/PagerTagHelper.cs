
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using mostlylucid.pagingtaghelper.Models;

namespace mostlylucid.pagingtaghelper.Components;

[HtmlTargetElement("paging", TagStructure = TagStructure.NormalOrSelfClosing)]
public class PagerTagHelper : TagHelper
{
  [HtmlAttributeName("model")] public IPagingModel? Model { get; set; }
  
  
  [HtmlAttributeName("search-term")] public string? SearchTerm { get; set; }
    // Required attributes
    [HtmlAttributeName("link-url")] public string? LinkUrl { get; set; }

    [HtmlAttributeName("page")] public int? Page { get; set; }

    [HtmlAttributeName("page-size")] public int? PageSize { get; set; }

    [HtmlAttributeName("total-items")] public int? TotalItems { get; set; }

    // Optional attributes with defaults
    [HtmlAttributeName("pages-to-display")]
    public int PagesToDisplay { get; set; } = 5;

    [HtmlAttributeName("css-class")] public string CssClass { get; set; } = "btn-group";

    [HtmlAttributeName("first-page-text")] public string FirstPageText { get; set; } = "«";

    [HtmlAttributeName("previous-page-text")]
    public string PreviousPageText { get; set; } = "‹ Previous";

    [HtmlAttributeName("skip-back-text")] public string SkipBackText { get; set; } = "..";

    [HtmlAttributeName("skip-forward-text")]
    public string SkipForwardText { get; set; } = "..";

    [HtmlAttributeName("next-page-text")] public string NextPageText { get; set; } = "Next ›";

    [HtmlAttributeName("next-page-aria-label")]
    public string NextPageAriaLabel { get; set; } = "go to next page";

    [HtmlAttributeName("last-page-text")] public string LastPageText { get; set; } = "»";

    [HtmlAttributeName("first-last-navigation")]
    public bool FirstLastNavigation { get; set; } = true;

    [HtmlAttributeName("skip-forward-back-navigation")]
    public bool SkipForwardBackNavigation { get; set; } = true;

    [HtmlAttributeName("htmx-target")] public string HtmxTarget { get; set; } = "";

    // Inject the current ViewContext
    [ViewContext] [HtmlAttributeNotBound] public required ViewContext ViewContext { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        // Remove the original <pager> tag from output
        output.TagName = "div";

        // Optionally, remove attributes that you don’t want to output if needed:
         output.Attributes.RemoveAll("page");
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
         

        // Get the view component helper and contextualize it
        var viewComponentHelper =
            (IViewComponentHelper)ViewContext.HttpContext.RequestServices.GetService(typeof(IViewComponentHelper))!;
        ((IViewContextAware)viewComponentHelper).Contextualize(ViewContext);

        // Invoke the "Pager" view component with the passed parameters
        var result = await viewComponentHelper.InvokeAsync("Pager", new
       PagerViewModel() {
           Model = Model,
           LinkUrl = LinkUrl,
            Page = Page,
           PageSize = PageSize,
           TotalItems = TotalItems,
           PagesToDisplay = PagesToDisplay,
          CssClass  = CssClass,
           FirstPageText = FirstPageText,
           PreviousPageText = PreviousPageText,
            SkipBackText = SkipBackText,
           SkipForwardText = SkipForwardText,
            NextPageText = NextPageText,
            NextPageAriaLabel = NextPageAriaLabel,
           LastPageText = LastPageText,
            FirstLastNavigation = FirstLastNavigation,
           SkipForwardBackNavigation = SkipForwardBackNavigation,
           HtmxTarget = HtmxTarget
        });

        // Write the rendered HTML from the view component into the output
        output.Content.SetHtmlContent(result);
    }
}