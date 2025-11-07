using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using mostlylucid.pagingtaghelper.Models;
using mostlylucid.pagingtaghelper.Models.TagModels;
using mostlylucid.pagingtaghelper.Services;

namespace mostlylucid.pagingtaghelper.Components;

/// <summary>
/// A tag helper that renders a page-size dropdown control using a view component.
/// </summary>
[HtmlTargetElement("page-size", TagStructure = TagStructure.NormalOrSelfClosing)]
public class PageSizeTagHelper(IUrlHelperFactory urlHelperFactory) : TagHelper
{
    private IUrlHelper UrlHelper => urlHelperFactory.GetUrlHelper(ViewContext);


    /// <summary>
    /// The IPagingModel to use for the page-size control. (Optional)
    /// </summary>
    [HtmlAttributeName("model")]
    public IPagingModel? Model { get; set; }

    /// <summary>
    /// Lets you define the page size steps to use in the dropdown. (Default: "10, 25, 50, 75, 100, 125, 150, 200, 250, 500, 1000")
    /// </summary>
    [HtmlAttributeName("page-size-steps")]
    public string? PageSizeSteps { get; set; }

    /// <summary>
    /// The maximum page size allowed in the dropdown. (Default: 1000)
    /// </summary>
    [HtmlAttributeName("max-page-size")]
    public int MaxPageSize { get; set; } = 1000;

    /// <summary>
    /// The view type to use for the page-size control. (Default: TailwindAndDaisy)
    /// </summary>
    [HtmlAttributeName("view-type")]
    public ViewType ViewType { get; set; } = ViewType.TailwindAndDaisy;

    /// <summary>
    /// Enable HTMX usage (default: true). Deprecated: use js-mode instead.
    /// </summary>
    [Obsolete("use-htmx is deprecated. Use js-mode=\"HTMX\" instead. This attribute will be removed in v2.0.", false)]
    [HtmlAttributeName("use-htmx")]
    public bool UseHtmx { get; set; } = true;

    /// <summary>
    /// JavaScript framework mode (HTMX, HTMXWithAlpine, Alpine, PlainJS, NoJS). If not set, derives from use-htmx.
    /// </summary>
    [HtmlAttributeName("js-mode")]
    public JavaScriptMode? JSMode { get; set; }

    /// <summary>
    /// The client-side id of the pager control. (Optional)
    /// </summary>
    [HtmlAttributeName("id")]
    public string? PagerId { get; set; }

    /// <summary>
    /// The page-size model to use for the control. (Optional)
    /// </summary>
    [HtmlAttributeName("page-size-model")]
    public PageSizeModel? PageSizeModel { get; set; }

    /// <summary>
    /// Hard-coded link URL to use for the page-size control. (Optional)
    /// </summary>
    [HtmlAttributeName("link-url")]
    public string? LinkUrl { get; set; }

    /// <summary>
    /// Alias for link-url, so you can write href="..." in your Razor. (Optional)
    /// </summary>
    [HtmlAttributeName("href")]
    public string? Href
    {
        get => LinkUrl;
        set => LinkUrl = value;
    }

    /// <summary>
    /// For ASP.NET Core MVC, the action to use for the page-size control. (Optional)
    /// </summary>
    [AspMvcAction]
    [HtmlAttributeName("action")]
    public string? Action { get; set; }

    /// <summary>
    /// For ASP.NET Core MVC, the controller to use for the page-size control. (Optional)
    /// </summary>
    [AspMvcController]
    [HtmlAttributeName("controller")]
    public string? Controller { get; set; }

    /// <summary>
    /// Default page size to use for the pager control. (Default: 10)
    /// </summary>
    [HtmlAttributeName("page-size")]
    public int? PageSize { get; set; }

    /// <summary>
    /// Whether to use a local (custom) view for the page-size control. (Default: false)
    /// </summary>
    [HtmlAttributeName("use-local-view")]
    public bool UseLocalView { get; set; } = false;

    /// <summary>
    /// Total number of items to paginate. (Optional)
    /// </summary>
    [HtmlAttributeName("total-items")]
    public int? TotalItems { get; set; }

    /// <summary>
    /// Language/culture code for localization (e.g., "en", "fr", "de"). If not specified, uses current UI culture.
    /// </summary>
    [HtmlAttributeName("language")]
    public string? Language { get; set; }

    /// <summary>
    /// Captures the current ViewContext so we can render the view component.
    /// </summary>
    [ViewContext, HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        // We want to render a <div> by default
        output.TagName = "div";

        // Remove any leftover attributes that we handle ourselves
        RemoveUnwantedAttributes(output);

        // Determine final PagerId
        var pagerId = PagerId ?? PageSizeModel?.PagerId ?? $"pager-{Guid.NewGuid():N}";
        // Assign ID to the outer div
        output.Attributes.SetAttribute("id", pagerId);
        // Build or fallback to a link URL
        var finalLinkUrl = BuildLinkUrl();
        if (string.IsNullOrEmpty(finalLinkUrl))
        {
            // If we can't build a URL, show a fallback message or short-circuit
            output.Content.SetHtmlContent(
                "<p style=\"color:red\">No valid link URL found for PageSize control.</p>");
            return;
        }

        var finalPageSize = PageSize ?? PageSizeModel?.PageSize ?? 10;
        // Clamp the page size to MaxPageSize

        var finalTotalItems = TotalItems ?? Model?.TotalItems ?? PageSizeModel?.TotalItems ?? 0;
        
        if (finalTotalItems == 0)
        {
            output.Content.SetHtmlContent("<p>No items found.</p>");
            return;
        }
        var maxPageSize = Math.Min(finalTotalItems, MaxPageSize);

        // Fallback to model's properties if not set
        var finalViewType = PageSizeModel?.ViewType ?? Model?.ViewType ?? ViewType;

        var pageSizeSteps = new int[] { 10, 25, 50, 75, 100, 125, 150, 200, 250, 500, 1000 };

        
        if (!string.IsNullOrEmpty(PageSizeSteps))
            pageSizeSteps = PageSizeSteps.Split(',').Select(s =>

            {
                if (int.TryParse(s, out var result))
                    return result;
                else
                {
                    throw new ArgumentException("Invalid page size step", nameof(PageSizeSteps));
                }
            }).ToArray();


        var pageSizes = CalculatePageSizes(finalTotalItems, maxPageSize, pageSizeSteps);

        var useHtmx = PageSizeModel?.UseHtmx ?? UseHtmx;
        var useLocalView = PageSizeModel?.UseLocalView ?? UseLocalView;

        // Acquire the IViewComponentHelper
        var viewComponentHelper = ViewContext.HttpContext.RequestServices
            .GetRequiredService<IViewComponentHelper>();
        ((IViewContextAware)viewComponentHelper).Contextualize(ViewContext);

        // Create localizer instance and set culture if specified
        var localizer = new PagingLocalizer();
        if (!string.IsNullOrEmpty(Language))
        {
            try
            {
                localizer.SetCulture(new System.Globalization.CultureInfo(Language));
            }
            catch (System.Globalization.CultureNotFoundException)
            {
                // Fallback to current culture if invalid language code provided
            }
        }

        // Construct the PageSizeModel (if not provided) with final settings
        var pagerModel = new PageSizeModel
        {
            ViewType = finalViewType,
            UseLocalView = useLocalView,
#pragma warning disable CS0618 // Type or member is obsolete
            UseHtmx = useHtmx,
#pragma warning restore CS0618 // Type or member is obsolete
            JSMode = JSMode,
            PageSizes = pageSizes,
            PagerId = pagerId,
            Model = Model,
            LinkUrl = finalLinkUrl,
            MaxPageSize = maxPageSize,
            PageSize = finalPageSize,
            TotalItems = finalTotalItems,
            Localizer = localizer
        };

        // Safely invoke the "PageSize" view component
        try
        {
            var result = await viewComponentHelper.InvokeAsync("PageSize", pagerModel);
            output.Content.SetHtmlContent(result);
        }
        catch (Exception ex)
        {
            // Optional: Log or display an error
            output.Content.SetHtmlContent(
                $"<p class=\"text-red-500\">Failed to render PageSize component: {ex.Message}</p>"
            );
        }
    }

    /// <summary>
    /// Removes attributes we explicitly handle in code, to keep the final markup clean.
    /// </summary>
    private void RemoveUnwantedAttributes(TagHelperOutput output)
    {
        var attributesToRemove = new[]
        {
            "page", "link-url", "page-size", "total-items", "pages-to-display",
            "model", "page-size-model", "use-local-view", "action", "controller",
            "view-type", "max-page-size", "use-htmx","page-size-steps", "language"
        };

        foreach (var attr in attributesToRemove)
        {
            if (output.Attributes.ContainsName(attr))
            {
                output.Attributes.RemoveAll(attr);
            }
        }
    }

    /// <summary>
    /// Builds a link URL from the given attributes or falls back to the current request path.
    /// </summary>
    private string? BuildLinkUrl()
    {
        // Priority 1: link-url / href
        var linkUrl = LinkUrl ?? PageSizeModel?.LinkUrl;
        if (!string.IsNullOrEmpty(linkUrl))
        {
            return linkUrl;
        }

        // Priority 2: If we have an action and controller, build the route
        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(Controller))
        {
            return UrlHelper.ActionLink(Action, Controller);
        }

        // Priority 3: Fallback to the current request path
        var fallbackUrl = ViewContext.HttpContext.Request.Path.ToString();
        return !string.IsNullOrEmpty(fallbackUrl) ? fallbackUrl : null;
    }

    private List<int> CalculatePageSizes(int totalItems, int maxxPageSize, int[] pageSizeSteps)
    {
        var pageSizes = new List<int>();

        // 1. Include all fixed steps up to both TotalItems and MaxPageSize
        foreach (var step in pageSizeSteps)
        {
            if (step <= totalItems && step <= maxxPageSize)
                pageSizes.Add(step);
        }

        // 2. If TotalItems exceeds the largest fixed step, keep doubling
        int lastFixedStep = pageSizeSteps.Last();
        if (totalItems > lastFixedStep)
        {
            int next = lastFixedStep;
            while (next < totalItems && next < maxxPageSize)
            {
                next *= 2; // double the step
                if (next <= totalItems && next <= maxxPageSize)
                {
                    pageSizes.Add(next);
                }
            }
        }

        // 3. Include TotalItems if it's not already in the list
        if (totalItems <= maxxPageSize && !pageSizes.Contains(totalItems))
        {
            pageSizes.Add(totalItems);
        }

        pageSizes.Sort();

        return pageSizes;
    }
}