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
/// Renders a pagination control via a ViewComponent.
/// </summary>
[HtmlTargetElement("paging", TagStructure = TagStructure.NormalOrSelfClosing)]
public class PagerTagHelper(IUrlHelperFactory urlHelperFactory) : TagHelper
{
    private IUrlHelper UrlHelper => urlHelperFactory.GetUrlHelper(ViewContext);
    /// <summary>Paging model containing pagination details (optional).</summary>
    [HtmlAttributeName("model")]
    public IPagingModel? Model { get; set; }

    /// <summary>
    /// The maximum page size allowed in the dropdown. (Default: 1000)
    /// </summary>
    [HtmlAttributeName("max-page-size")]
    public int MaxPageSize { get; set; } = 1000;
    
    /// <summary>Determines the UI style (default: TailwindANdDaisy).</summary>
    [HtmlAttributeName("view-type")]
    public ViewType ViewType { get; set; } = ViewType.TailwindAndDaisy;

    /// <summary>Enable HTMX usage (default: true). Deprecated: use js-mode instead.</summary>
    [Obsolete("use-htmx is deprecated. Use js-mode=\"HTMX\" instead. This attribute will be removed in v2.0.", false)]
    [HtmlAttributeName("use-htmx")]
    public bool UseHtmx { get; set; } = true;

    /// <summary>JavaScript framework mode (HTMX, HTMXWithAlpine, Alpine, PlainJS, NoJS). If not set, derives from use-htmx.</summary>
    [HtmlAttributeName("js-mode")]
    public JavaScriptMode? JSMode { get; set; }

    /// <summary>Optional custom ID for the pager container.</summary>
    [HtmlAttributeName("id")]
    public string? PagerId { get; set; }

    /// <summary>Optional pre-configured PagerViewModel.</summary>
    [HtmlAttributeName("pagingmodel")]
    public PagerViewModel? PagerModel { get; set; }

    /// <summary>Whether to show the page-size dropdown (default: true).</summary>
    [HtmlAttributeName("show-pagesize")]
    public bool ShowPageSize { get; set; } = true;

    /// <summary>Search term used for filtering.</summary>
    [HtmlAttributeName("search-term")]
    public string? SearchTerm { get; set; }

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

    /// <summary>Current page number.</summary>
    [HtmlAttributeName("page")]
    public int? Page { get; set; }

    /// <summary>Number of items per page.</summary>
    [HtmlAttributeName("page-size")]
    public int? PageSize { get; set; }

    /// <summary>Use a local (custom) view or the built-in one (default: false).</summary>
    [HtmlAttributeName("use-local-view")]
    public bool UseLocalView { get; set; } = false;

    /// <summary>Total number of items in the data set.</summary>
    [HtmlAttributeName("total-items")]
    public int? TotalItems { get; set; }

    /// <summary>Number of pages to display at once (default: 5).</summary>
    [HtmlAttributeName("pages-to-display")]
    public int PagesToDisplay { get; set; } = 5;

    /// <summary>CSS class for the pager container (default: "btn-group").</summary>
    [HtmlAttributeName("css-class")]
    public string CssClass { get; set; } = "btn-group";

    /// <summary>Text for the first page link (default: "«").</summary>
    [HtmlAttributeName("first-page-text")]
    public string FirstPageText { get; set; } = "«";

    /// <summary>Text for the previous page link (default: "‹ Previous").</summary>
    [HtmlAttributeName("previous-page-text")]
    public string PreviousPageText { get; set; } = "‹ Previous";

    /// <summary>Text for skipping backward in pagination (default: "..").</summary>
    [HtmlAttributeName("skip-back-text")]
    public string SkipBackText { get; set; } = "..";

    /// <summary>Text for skipping forward in pagination (default: "..").</summary>
    [HtmlAttributeName("skip-forward-text")]
    public string SkipForwardText { get; set; } = "..";

    /// <summary>Text for the next page link (default: "Next ›").</summary>
    [HtmlAttributeName("next-page-text")]
    public string NextPageText { get; set; } = "Next ›";

    /// <summary>ARIA label for the next page link (default: "go to next page").</summary>
    [HtmlAttributeName("next-page-aria-label")]
    public string NextPageAriaLabel { get; set; } = "go to next page";

    /// <summary>Text for the last page link (default: "»").</summary>
    [HtmlAttributeName("last-page-text")]
    public string LastPageText { get; set; } = "»";

    /// <summary>Show first/last page navigation links (default: true).</summary>
    [HtmlAttributeName("first-last-navigation")]
    public bool FirstLastNavigation { get; set; } = true;

    /// <summary>Show skip forward/backward links (default: true).</summary>
    [HtmlAttributeName("skip-forward-back-navigation")]
    public bool SkipForwardBackNavigation { get; set; } = true;

    /// <summary>HTMX target for AJAX-based pagination.</summary>
    [HtmlAttributeName("htmx-target")]
    public string HtmxTarget { get; set; } = string.Empty;

    /// <summary>Enable descending sort (optional).</summary>
    [HtmlAttributeName("descending")]
    public bool? Descending { get; set; }

    /// <summary>Property name used for sorting (optional).</summary>
    [HtmlAttributeName("order-by")]
    public string? OrderBy { get; set; }

    /// <summary>Whether to show the pagination summary text (default: true).</summary>
    [HtmlAttributeName("show-summary")]
    public bool ShowSummary { get; set; } = true;

    /// <summary>
    /// Custom template for the page summary. Supports placeholders: {currentPage}, {totalPages}, {totalItems}, {pageSize}, {startItem}, {endItem}.
    /// Example: "Showing {startItem}-{endItem} of {totalItems} items"
    /// </summary>
    [HtmlAttributeName("summary-template")]
    public string? SummaryTemplate { get; set; }

    /// <summary>
    /// Language/culture code for localization (e.g., "en", "fr", "de"). If not specified, auto-detects from browser Accept-Language header or uses current UI culture.
    /// </summary>
    [HtmlAttributeName("language")]
    public string? Language { get; set; }

    /// <summary>Captures the current ViewContext (injected by the runtime).</summary>
    [ViewContext, HtmlAttributeNotBound]
    public required ViewContext ViewContext { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        // Render as a <div> by default
        output.TagName = "div";

        // Remove attributes we handle ourselves
        RemoveUnwantedAttributes(output);

        // Generate or reuse a pager ID
        var finalPagerId = PagerId ?? $"pager-{Guid.NewGuid():N}";

        // Derive linkUrl or fallback to current request path
        var finalLinkUrl = LinkUrl ?? ViewContext.HttpContext.Request.Path;

        // Merge in any values from the IPagingModel if present
        PageSize = Model?.PageSize ?? PageSize ?? 10;
        Page     = Model?.Page     ?? Page     ?? 1;
        ViewType = Model?.ViewType ?? ViewType;
        TotalItems = Model?.TotalItems ?? TotalItems ?? 0;

        // If the model is IPagingSearchModel, sync SearchTerm
        if (Model is IPagingSearchModel searchModel)
        {
            SearchTerm = searchModel.SearchTerm ?? SearchTerm;
        }

        // Assign ID to the container
        output.Attributes.SetAttribute("id", finalPagerId);

        // Acquire the IViewComponentHelper
        var services = ViewContext.HttpContext.RequestServices;
        var vcHelper = (IViewComponentHelper)services.GetRequiredService(typeof(IViewComponentHelper));
        ((IViewContextAware)vcHelper).Contextualize(ViewContext);

        // Create localizer instance and set culture
        var localizer = new PagingLocalizer();
        var effectiveLanguage = Language ?? DetectLanguage(ViewContext);

        if (!string.IsNullOrEmpty(effectiveLanguage))
        {
            try
            {
                localizer.SetCulture(new System.Globalization.CultureInfo(effectiveLanguage));
            }
            catch (System.Globalization.CultureNotFoundException)
            {
                // Fallback to English for unsupported languages
                try
                {
                    localizer.SetCulture(new System.Globalization.CultureInfo("en"));
                }
                catch
                {
                    // Use default if even "en" fails
                }
            }
        }

        // Build or reuse the PagerViewModel
        var pagerModel = PagerModel ?? new PagerViewModel
        {
            OrderBy                    = OrderBy,
            Descending                 = Descending,
            ShowSummary                = ShowSummary,
            ViewType                   = ViewType,
            UseLocalView               = UseLocalView,
#pragma warning disable CS0618 // Type or member is obsolete
            UseHtmx                    = UseHtmx,
#pragma warning restore CS0618 // Type or member is obsolete
            JSMode                     = JSMode,
            PagerId                    = finalPagerId,
            SearchTerm                 = SearchTerm,
            ShowPageSize               = ShowPageSize,
            Model                      = Model,
            LinkUrl                    = finalLinkUrl,
            MaxPageSize                = MaxPageSize,
            Page                       = Page.Value,
            PageSize                   = PageSize.Value,
            TotalItems                 = TotalItems.Value,
            PagesToDisplay             = PagesToDisplay,
            CssClass                   = CssClass,
            FirstPageText              = FirstPageText,
            PreviousPageText           = PreviousPageText,
            SkipBackText               = SkipBackText,
            SkipForwardText            = SkipForwardText,
            NextPageText               = NextPageText,
            NextPageAriaLabel          = NextPageAriaLabel,
            LastPageText               = LastPageText,
            FirstLastNavigation        = FirstLastNavigation,
            SkipForwardBackNavigation  = SkipForwardBackNavigation,
            HtmxTarget                 = HtmxTarget,
            Localizer                  = localizer,
            SummaryTemplate            = SummaryTemplate
        };

        // Safely invoke the "Pager" ViewComponent
        try
        {
            var result = await vcHelper.InvokeAsync("Pager", pagerModel);
            output.Content.SetHtmlContent(result);
        }
        catch (Exception ex)
        {
            // Optional: log the exception or show a user-friendly message
            output.Content.SetHtmlContent(
                $"<div style=\"color:red\">Error rendering pager: {ex.Message}</div>"
            );
        }
    }

    /// <summary>
    /// Builds a link URL from the given attributes or falls back to the current request path.
    /// </summary>
    private string? BuildLinkUrl()
    {
        // Priority 1: link-url / href
        var linkUrl = LinkUrl ??  PagerModel?.LinkUrl;
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
    
    private static void RemoveUnwantedAttributes(TagHelperOutput output)
    {
        var attrs = new[]
        {
            "page", "link-url", "page-size", "total-items", "pages-to-display",
            "css-class", "first-page-text", "previous-page-text", "skip-back-text",
            "skip-forward-text", "next-page-text", "next-page-aria-label", "last-page-text",
            "first-last-navigation", "skip-forward-back-navigation", "model", "show-pagesize",
            "pagingmodel", "use-local-view", "search-term", "htmx-target", "descending",
            "order-by", "show-summary", "summary-template", "language"
        };
        foreach (var attr in attrs)
        {
            output.Attributes.RemoveAll(attr);
        }
    }

    /// <summary>
    /// Detects the language from browser Accept-Language header or current culture.
    /// </summary>
    private static string? DetectLanguage(ViewContext viewContext)
    {
        // First try: Browser's Accept-Language header
        var acceptLanguage = viewContext.HttpContext.Request.Headers["Accept-Language"].FirstOrDefault();
        if (!string.IsNullOrEmpty(acceptLanguage))
        {
            // Parse the first language preference (e.g., "en-US,en;q=0.9,fr;q=0.8" -> "en-US")
            var firstLang = acceptLanguage.Split(',').FirstOrDefault()?.Split(';').FirstOrDefault()?.Trim();
            if (!string.IsNullOrEmpty(firstLang))
            {
                // Try to get just the language code (e.g., "en-US" -> "en")
                var langCode = firstLang.Split('-').FirstOrDefault();

                // Check if it's a supported language
                var supportedLanguages = new[] { "en", "de", "es", "fr", "it", "pt", "ja", "zh-Hans" };
                if (supportedLanguages.Contains(langCode))
                {
                    return langCode;
                }

                // Special case for Chinese
                if (firstLang.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
                {
                    return "zh-Hans"; // Simplified Chinese
                }
            }
        }

        // Second try: Current UI culture
        var currentCulture = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        return currentCulture != "iv" ? currentCulture : "en"; // "iv" is invariant culture, default to English
    }
}