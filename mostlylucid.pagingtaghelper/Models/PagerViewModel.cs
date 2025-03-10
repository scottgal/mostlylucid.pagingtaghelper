using mostlylucid.pagingtaghelper.Models;

namespace mostlylucid.pagingtaghelper.Components;

public class PagerViewModel
{
    public IPagingModel? Model { get; set; }

    public bool UseLocalView { get; set; } = false;
    
    public bool UseHtmx { get; set; } = true;

    public string? PagerId { get; set; }
    public bool ShowPageSize { get; set; } = true;
        
    public string? SearchTerm { get; set; }
    // Required properties
    public string? LinkUrl { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public int? TotalItems { get; set; }

    // Optional properties with defaults (using DaisyUI/Tailwind classes)
    public int PagesToDisplay { get; set; } = 5;
    public string CssClass { get; set; } = "btn-group";  // DaisyUI grouping style
    public string FirstPageText { get; set; } = "«";
    public string PreviousPageText { get; set; } = "‹ Previous";
    public string SkipBackText { get; set; } = "..";
    public string SkipForwardText { get; set; } = "..";
    public string NextPageText { get; set; } = "Next ›";
    public string NextPageAriaLabel { get; set; } = "go to next page";
    public string LastPageText { get; set; } = "»";
    public bool FirstLastNavigation { get; set; } = true;
    public bool SkipForwardBackNavigation { get; set; } = true;

    // Calculated value
    public int TotalPages { get; set; }

    // Optional htmx integration:
    // If set (e.g. "#content"), pagination links will include htmx attributes.
    public string HtmxTarget { get; set; } = "";
}