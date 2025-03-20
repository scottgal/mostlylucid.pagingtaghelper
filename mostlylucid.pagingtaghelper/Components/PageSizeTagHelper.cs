using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using mostlylucid.pagingtaghelper.Models;
using mostlylucid.pagingtaghelper.Models.TagModels;

namespace mostlylucid.pagingtaghelper.Components;

/// <summary>
/// A tag helper that renders a pagination control using a view component.
/// </summary>
[HtmlTargetElement("page-size", TagStructure = TagStructure.NormalOrSelfClosing)]
public class PageSizeTagHelper : TagHelper
{
    [HtmlAttributeName("model")]
    public IPagingModel? Model { get; set; }

    [HtmlAttributeName("view-type")]
    public ViewType ViewType { get; set; } = ViewType.TailwindANdDaisy;

    [HtmlAttributeName("use-htmx")]
    public bool UseHtmx { get; set; } = true;

    [HtmlAttributeName("id")]
    public string? PagerId { get; set; }

    [HtmlAttributeName("page-size-model")]
    public PageSizeModel? PageSizeModel { get; set; } = null;

    [HtmlAttributeName("search-term")]
    public string? SearchTerm { get; set; }

    [HtmlAttributeName("link-url")]
    public string? LinkUrl { get; set; }

    [HtmlAttributeName("page")]
    public int? Page { get; set; }

    [HtmlAttributeName("page-size")]
    public int? PageSize { get; set; }

    [HtmlAttributeName("use-local-view")]
    public bool UseLocalView { get; set; } = false;

    [HtmlAttributeName("total-items")]
    public int? TotalItems { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";

        // Remove unnecessary attributes
        string[] attributesToRemove = {
            "page", "link-url", "page-size", "total-items",
            "pages-to-display", "model", "page-size-model",
            "use-local-view"
        };
        foreach (var attr in attributesToRemove)
        {
            if (output.Attributes.ContainsName(attr))
            {
                output.Attributes.RemoveAll(attr);
            }
        }

        var pagerId = PagerId ?? PageSizeModel?.PagerId ?? $"pager-{Guid.NewGuid():N}";
        var linkUrl = LinkUrl ?? PageSizeModel?.LinkUrl ?? ViewContext.HttpContext.Request.Path;
        PageSize ??= Model?.PageSize ?? 10;
        Page ??= Model?.Page ?? 1;
        ViewType = Model?.ViewType ?? ViewType;
        TotalItems ??= Model?.TotalItems ?? 0;
        if (Model is IPagingSearchModel searchModel)
            SearchTerm = searchModel?.SearchTerm ?? SearchTerm ?? "";

        output.Attributes.SetAttribute("id", pagerId);

        // Get ViewComponentHelper
        var viewComponentHelper = ViewContext.HttpContext.RequestServices
            .GetRequiredService<IViewComponentHelper>();
        ((IViewContextAware)viewComponentHelper).Contextualize(ViewContext);

        var pagerModel = PageSizeModel ?? new PageSizeModel
        {
            ViewType = ViewType,
            UseLocalView = UseLocalView,
            UseHtmx = UseHtmx,
            PagerId = pagerId,
            SearchTerm = SearchTerm,
            Model = Model,
            LinkUrl = linkUrl,
            Page = Page.Value,
            PageSize = PageSize.Value,
            TotalItems = TotalItems.Value
        };

        var result = await viewComponentHelper.InvokeAsync("PageSize", pagerModel);
        output.Content.SetHtmlContent(result);
    }
}