using mostlylucid.pagingtaghelper.Services;

namespace mostlylucid.pagingtaghelper.Models.TagModels;

public class PagerViewModel : PageSizeModel
{
    private string? _firstPageText;
    private string? _previousPageText;
    private string? _nextPageText;
    private string? _lastPageText;
    private string? _skipBackText;
    private string? _skipForwardText;
    private string? _nextPageAriaLabel;
    private string? _pageSizeString;

    public bool? Descending { get; set; }

    public string? OrderBy { get; set; }

    public bool ShowSummary { get; set; } = true;

    public bool ShowPageSize { get; set; } = true;

    public int PagesToDisplay { get; set; } = 5;
    public string CssClass { get; set; } = "btn-group"; // DaisyUI grouping style

    public string FirstPageText
    {
        get => _firstPageText ?? Localizer?.FirstPageText ?? "«";
        set => _firstPageText = value;
    }

    public string PreviousPageText
    {
        get => _previousPageText ?? Localizer?.PreviousPageText ?? "‹ Previous";
        set => _previousPageText = value;
    }

    public string SkipBackText
    {
        get => _skipBackText ?? Localizer?.SkipBackText ?? "..";
        set => _skipBackText = value;
    }

    public string SkipForwardText
    {
        get => _skipForwardText ?? Localizer?.SkipForwardText ?? "..";
        set => _skipForwardText = value;
    }

    public string NextPageText
    {
        get => _nextPageText ?? Localizer?.NextPageText ?? "Next ›";
        set => _nextPageText = value;
    }

    public string NextPageAriaLabel
    {
        get => _nextPageAriaLabel ?? Localizer?.NextPageAriaLabel ?? "go to next page";
        set => _nextPageAriaLabel = value;
    }

    public string PageSizeString
    {
        get => _pageSizeString ?? Localizer?.PageSizeLabel ?? "Page size:";
        set => _pageSizeString = value;
    }


    public int Page { get; set; }

    public int TotalPages => (int)Math.Ceiling(TotalItems! / (double)PageSize!);

    public string LastPageText
    {
        get => _lastPageText ?? Localizer?.LastPageText ?? "»";
        set => _lastPageText = value;
    }

    public bool FirstLastNavigation { get; set; } = true;
    public bool SkipForwardBackNavigation { get; set; } = true;

    // Optional htmx integration:
    // If set (e.g. "#content"), pagination links will include htmx attributes.
    public string HtmxTarget { get; set; } = "";

    /// <summary>
    /// Custom template for the page summary. Supports placeholders: {currentPage}, {totalPages}, {totalItems}, {pageSize}, {startItem}, {endItem}.
    /// Example: "Showing {startItem}-{endItem} of {totalItems} items"
    /// </summary>
    public string? SummaryTemplate { get; set; }

    /// <summary>
    /// Gets the localized page summary text
    /// </summary>
    public string GetPageSummary()
    {
        // If custom template is provided, use it with placeholder replacement
        if (!string.IsNullOrEmpty(SummaryTemplate))
        {
            int startItem = (Page - 1) * PageSize + 1;
            int endItem = Math.Min(Page * PageSize, TotalItems);

            return SummaryTemplate
                .Replace("{currentPage}", Page.ToString())
                .Replace("{totalPages}", TotalPages.ToString())
                .Replace("{totalItems}", TotalItems.ToString())
                .Replace("{pageSize}", PageSize.ToString())
                .Replace("{startItem}", startItem.ToString())
                .Replace("{endItem}", endItem.ToString());
        }

        // Otherwise use localizer or default
        if (Localizer != null)
        {
            return Localizer.GetPageSummary(Page, TotalPages, TotalItems);
        }
        return $"Page {Page} of {TotalPages} (Total items: {TotalItems})";
    }

    public int StartPage
    {
        get
        {
            int halfDisplay = PagesToDisplay / 2;
            int startPage = Math.Max(1, Page - halfDisplay);
            int endPage = Math.Min(TotalPages, startPage + PagesToDisplay - 1);
            return Math.Max(1, endPage - PagesToDisplay + 1);
        }
    }

    public int EndPage
    {
        get
        {
            int halfDisplay = PagesToDisplay / 2;
            int startPage = Math.Max(1, Page - halfDisplay);
            return Math.Min(TotalPages, startPage + PagesToDisplay - 1);
        }
    }
}