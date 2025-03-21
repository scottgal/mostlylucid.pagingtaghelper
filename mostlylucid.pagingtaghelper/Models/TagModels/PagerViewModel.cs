namespace mostlylucid.pagingtaghelper.Models.TagModels;

public class PagerViewModel : PageSizeModel
{
    public bool? Descending { get; set; }

    public string? OrderBy { get; set; }
    
    public bool ShowSummary { get; set; } = true;

    public bool ShowPageSize { get; set; } = true;

    public int PagesToDisplay { get; set; } = 5;
    public string CssClass { get; set; } = "btn-group"; // DaisyUI grouping style
    public string FirstPageText { get; set; } = "«";
    public string PreviousPageText { get; set; } = "‹ Previous";
    public string SkipBackText { get; set; } = "..";
    public string SkipForwardText { get; set; } = "..";
    public string NextPageText { get; set; } = "Next ›";
    public string NextPageAriaLabel { get; set; } = "go to next page";

    public string PageSizeString { get; set; } = "age size:";

    
    public int Page { get; set; }

    public int TotalPages => (int)Math.Ceiling(TotalItems! / (double)PageSize!);
    public string LastPageText { get; set; } = "»";
    public bool FirstLastNavigation { get; set; } = true;
    public bool SkipForwardBackNavigation { get; set; } = true;

    // Optional htmx integration:
    // If set (e.g. "#content"), pagination links will include htmx attributes.
    public string HtmxTarget { get; set; } = "";


    public int StartPage => Math.Max(1, Page - 2);
    public int EndPage => Math.Min(TotalPages, Page + 2);
}