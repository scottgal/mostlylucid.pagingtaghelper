using mostlylucid.pagingtaghelper.Services;

namespace mostlylucid.pagingtaghelper.Models.TagModels;

/// <summary>
/// ViewModel for the ContinuationPager ViewComponent.
/// Used for pagination with continuation tokens (Cosmos DB, Azure Table Storage, etc.)
/// </summary>
public class ContinuationPagerViewModel : PageSizeModel
{
    /// <summary>
    /// The continuation token for the next page.
    /// </summary>
    public string? NextPageToken { get; set; }

    /// <summary>
    /// Indicates whether there are more results available.
    /// </summary>
    public bool HasMoreResults { get; set; }

    /// <summary>
    /// Current page number for display purposes (1-based).
    /// </summary>
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// Dictionary storing page tokens for navigation.
    /// Key = page number, Value = token to get to that page.
    /// </summary>
    public Dictionary<int, string>? PageTokenHistory { get; set; }

    /// <summary>
    /// Optional: The IContinuationPagingModel if provided.
    /// </summary>
    public new IContinuationPagingModel? Model { get; set; }

    /// <summary>
    /// Text for the "Previous" button.
    /// </summary>
    public string PreviousPageText { get; set; } = "‹ Previous";

    /// <summary>
    /// Text for the "Next" button.
    /// </summary>
    public string NextPageText { get; set; } = "Next ›";

    /// <summary>
    /// ARIA label for previous button.
    /// </summary>
    public string PreviousPageAriaLabel { get; set; } = "Go to previous page";

    /// <summary>
    /// ARIA label for next button.
    /// </summary>
    public string NextPageAriaLabel { get; set; } = "Go to next page";

    /// <summary>
    /// CSS class for the container.
    /// </summary>
    public string CssClass { get; set; } = "flex gap-2 items-center";

    /// <summary>
    /// HTMX target for AJAX updates.
    /// </summary>
    public string HtmxTarget { get; set; } = string.Empty;

    /// <summary>
    /// Whether to show the page size selector.
    /// </summary>
    public bool ShowPageSize { get; set; } = true;

    /// <summary>
    /// Whether to show the current page number.
    /// </summary>
    public bool ShowPageNumber { get; set; } = true;

    /// <summary>
    /// Whether to enable token accumulation for faster backward navigation.
    /// </summary>
    public bool EnableTokenAccumulation { get; set; } = true;

    /// <summary>
    /// Optional prefix for query parameters to support multiple pagers on the same page.
    /// Example: "products" will generate "products_pageToken", "products_currentPage", etc.
    /// </summary>
    public string? ParameterPrefix { get; set; }

    /// <summary>
    /// Gets the token for the previous page from history.
    /// </summary>
    public string? GetPreviousPageToken()
    {
        if (CurrentPage <= 1) return null;
        if (PageTokenHistory == null) return null;

        // The token stored at (CurrentPage - 1) gets us back to the previous page
        return PageTokenHistory.TryGetValue(CurrentPage - 1, out var token) ? token : null;
    }

    /// <summary>
    /// Checks if we can navigate to the previous page.
    /// </summary>
    public bool CanNavigatePrevious => CurrentPage > 1 && GetPreviousPageToken() != null;

    /// <summary>
    /// Checks if we can navigate to the next page.
    /// </summary>
    public bool CanNavigateNext => HasMoreResults && !string.IsNullOrEmpty(NextPageToken);

    /// <summary>
    /// Gets a parameter name with the prefix applied (if set).
    /// </summary>
    /// <param name="paramName">The base parameter name</param>
    /// <returns>The prefixed parameter name, or the base name if no prefix is set</returns>
    public string GetParameterName(string paramName)
    {
        return string.IsNullOrWhiteSpace(ParameterPrefix)
            ? paramName
            : $"{ParameterPrefix}_{paramName}";
    }
}
