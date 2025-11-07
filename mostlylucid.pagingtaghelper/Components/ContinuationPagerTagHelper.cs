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
/// Renders a continuation token-based pagination control (Previous/Next only).
/// Perfect for Cosmos DB, Azure Table Storage, AWS DynamoDB, etc.
/// </summary>
[HtmlTargetElement("continuation-pager", TagStructure = TagStructure.NormalOrSelfClosing)]
public class ContinuationPagerTagHelper(IUrlHelperFactory urlHelperFactory) : TagHelper
{
    private IUrlHelper UrlHelper => urlHelperFactory.GetUrlHelper(ViewContext);

    /// <summary>Continuation paging model containing token details (optional).</summary>
    [HtmlAttributeName("model")]
    public IContinuationPagingModel? Model { get; set; }

    /// <summary>Determines the UI style (default: TailwindAndDaisy).</summary>
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

    /// <summary>Optional pre-configured ContinuationPagerViewModel.</summary>
    [HtmlAttributeName("pager-model")]
    public ContinuationPagerViewModel? PagerModel { get; set; }

    /// <summary>Whether to show the page-size dropdown (default: true).</summary>
    [HtmlAttributeName("show-pagesize")]
    public bool ShowPageSize { get; set; } = true;

    /// <summary>Whether to show the current page number (default: true).</summary>
    [HtmlAttributeName("show-page-number")]
    public bool ShowPageNumber { get; set; } = true;

    /// <summary>Whether to show the pagination summary text (default: true).</summary>
    [HtmlAttributeName("show-summary")]
    public bool ShowSummary { get; set; } = true;

    /// <summary>Custom template for the page summary. Supports placeholders: {currentPage}, {pageSize}.</summary>
    [HtmlAttributeName("summary-template")]
    public string? SummaryTemplate { get; set; }

    /// <summary>Search term used for filtering.</summary>
    [HtmlAttributeName("search-term")]
    public string? SearchTerm { get; set; }

    /// <summary>Hard-coded link URL to use for navigation (Optional).</summary>
    [HtmlAttributeName("link-url")]
    public string? LinkUrl { get; set; }

    /// <summary>Alias for link-url, so you can write href="..." in your Razor (Optional).</summary>
    [HtmlAttributeName("href")]
    public string? Href
    {
        get => LinkUrl;
        set => LinkUrl = value;
    }

    /// <summary>For ASP.NET Core MVC, the action to use for navigation (Optional).</summary>
    [AspMvcAction]
    [HtmlAttributeName("action")]
    public string? Action { get; set; }

    /// <summary>For ASP.NET Core MVC, the controller to use for navigation (Optional).</summary>
    [AspMvcController]
    [HtmlAttributeName("controller")]
    public string? Controller { get; set; }

    /// <summary>The continuation token for the next page.</summary>
    [HtmlAttributeName("next-page-token")]
    public string? NextPageToken { get; set; }

    /// <summary>Indicates whether there are more results available.</summary>
    [HtmlAttributeName("has-more-results")]
    public bool? HasMoreResults { get; set; }

    /// <summary>Number of items per page.</summary>
    [HtmlAttributeName("page-size")]
    public int? PageSize { get; set; }

    /// <summary>Current page number (for display purposes).</summary>
    [HtmlAttributeName("current-page")]
    public int? CurrentPage { get; set; }

    /// <summary>Use a local (custom) view or the built-in one (default: false).</summary>
    [HtmlAttributeName("use-local-view")]
    public bool UseLocalView { get; set; } = false;

    /// <summary>CSS class for the pager container (default: "flex gap-2 items-center").</summary>
    [HtmlAttributeName("css-class")]
    public string CssClass { get; set; } = "flex gap-2 items-center";

    /// <summary>Text for the previous page link (default: "‹ Previous").</summary>
    [HtmlAttributeName("previous-page-text")]
    public string PreviousPageText { get; set; } = "‹ Previous";

    /// <summary>Text for the next page link (default: "Next ›").</summary>
    [HtmlAttributeName("next-page-text")]
    public string NextPageText { get; set; } = "Next ›";

    /// <summary>ARIA label for the previous page link (default: "go to previous page").</summary>
    [HtmlAttributeName("previous-page-aria-label")]
    public string PreviousPageAriaLabel { get; set; } = "go to previous page";

    /// <summary>ARIA label for the next page link (default: "go to next page").</summary>
    [HtmlAttributeName("next-page-aria-label")]
    public string NextPageAriaLabel { get; set; } = "go to next page";

    /// <summary>HTMX target for AJAX-based pagination.</summary>
    [HtmlAttributeName("htmx-target")]
    public string HtmxTarget { get; set; } = string.Empty;

    /// <summary>Language/culture code for localization (e.g., "en", "fr", "de"). If not specified, auto-detects from browser Accept-Language header or uses current UI culture.</summary>
    [HtmlAttributeName("language")]
    public string? Language { get; set; }

    /// <summary>Whether to enable token accumulation for faster backward navigation (default: true).</summary>
    [HtmlAttributeName("enable-token-accumulation")]
    public bool EnableTokenAccumulation { get; set; } = true;

    /// <summary>Maximum number of page tokens to store in history (default: 20, 0 for unlimited).</summary>
    [HtmlAttributeName("max-history-pages")]
    public int MaxHistoryPages { get; set; } = 20;

    /// <summary>Whether to preserve all URL query parameters when navigating (default: true).</summary>
    [HtmlAttributeName("preserve-query-parameters")]
    public bool PreserveQueryParameters { get; set; } = true;

    /// <summary>JSON-serialized dictionary of page tokens for backward navigation.</summary>
    [HtmlAttributeName("page-token-history")]
    public string? PageTokenHistoryJson { get; set; }

    /// <summary>Optional prefix for query parameters to support multiple pagers on the same page (e.g., "products" -> "products_pageToken").</summary>
    [HtmlAttributeName("parameter-prefix")]
    public string? ParameterPrefix { get; set; }

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
        var finalPagerId = PagerId ?? $"continuation-pager-{Guid.NewGuid():N}";

        // Derive linkUrl or fallback to current request path
        var finalLinkUrl = LinkUrl ?? ViewContext.HttpContext.Request.Path;

        // Merge in any values from the IContinuationPagingModel if present
        PageSize = Model?.PageSize ?? PageSize ?? 10;
        CurrentPage = Model?.CurrentPage ?? CurrentPage ?? 1;
        ViewType = Model?.ViewType ?? ViewType;
        NextPageToken = Model?.NextPageToken ?? NextPageToken;
        HasMoreResults = Model?.HasMoreResults ?? HasMoreResults ?? false;

        // Parse token history if provided
        Dictionary<int, string>? tokenHistory = null;
        if (Model?.PageTokenHistory != null)
        {
            tokenHistory = Model.PageTokenHistory;
        }
        else if (!string.IsNullOrEmpty(PageTokenHistoryJson))
        {
            try
            {
                tokenHistory = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, string>>(PageTokenHistoryJson);
            }
            catch
            {
                // Invalid JSON, ignore
            }
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

        // Build or reuse the ContinuationPagerViewModel
        var pagerModel = PagerModel ?? new ContinuationPagerViewModel
        {
            ViewType = ViewType,
            UseLocalView = UseLocalView,
#pragma warning disable CS0618 // Type or member is obsolete
            UseHtmx = UseHtmx,
#pragma warning restore CS0618 // Type or member is obsolete
            JSMode = JSMode,
            PagerId = finalPagerId,
            SearchTerm = SearchTerm,
            ShowPageSize = ShowPageSize,
            ShowPageNumber = ShowPageNumber,
            ShowSummary = ShowSummary,
            SummaryTemplate = SummaryTemplate,
            Model = Model,
            LinkUrl = finalLinkUrl,
            PageSize = PageSize.Value,
            CurrentPage = CurrentPage.Value,
            NextPageToken = NextPageToken,
            HasMoreResults = HasMoreResults.Value,
            PageTokenHistory = tokenHistory,
            CssClass = CssClass,
            PreviousPageText = PreviousPageText,
            NextPageText = NextPageText,
            PreviousPageAriaLabel = PreviousPageAriaLabel,
            NextPageAriaLabel = NextPageAriaLabel,
            HtmxTarget = HtmxTarget,
            Localizer = localizer,
            EnableTokenAccumulation = EnableTokenAccumulation,
            MaxHistoryPages = MaxHistoryPages,
            PreserveQueryParameters = PreserveQueryParameters,
            ParameterPrefix = ParameterPrefix
        };

        // Safely invoke the "ContinuationPager" ViewComponent
        try
        {
            var result = await vcHelper.InvokeAsync("ContinuationPager", pagerModel);
            output.Content.SetHtmlContent(result);
        }
        catch (Exception ex)
        {
            // Optional: log the exception or show a user-friendly message
            output.Content.SetHtmlContent(
                $"<div style=\"color:red\">Error rendering continuation pager: {ex.Message}</div>"
            );
        }
    }

    private static void RemoveUnwantedAttributes(TagHelperOutput output)
    {
        var attrs = new[]
        {
            "model", "view-type", "use-htmx", "js-mode", "pager-model", "show-pagesize",
            "show-page-number", "show-summary", "summary-template", "search-term", "link-url",
            "href", "action", "controller", "next-page-token", "has-more-results", "page-size",
            "current-page", "use-local-view", "css-class", "previous-page-text", "next-page-text",
            "previous-page-aria-label", "next-page-aria-label", "htmx-target", "language",
            "enable-token-accumulation", "max-history-pages", "preserve-query-parameters",
            "page-token-history", "parameter-prefix"
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
