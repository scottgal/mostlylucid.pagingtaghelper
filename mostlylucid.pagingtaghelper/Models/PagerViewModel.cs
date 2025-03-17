namespace mostlylucid.pagingtaghelper.Models;

public class PagerViewModel
{
    public IPagingModel? Model { get; set; }

    public ViewType ViewType { get; set; }
    public bool UseLocalView { get; set; } = false;
    
    public bool? Descending { get; set; }
    
    public string? OrderBy { get; set; }
    
    public bool UseHtmx { get; set; } = true;

    public bool ShowSummary { get; set; } = true;
    
    public string? PagerId { get; set; }
    public bool ShowPageSize { get; set; } = true;
        
    public string? SearchTerm { get; set; }
    // Required properties
    public string? LinkUrl { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    
    public int PagesToDisplay { get; set; } = 5;
    public string CssClass { get; set; } = "btn-group";  // DaisyUI grouping style
    public string FirstPageText { get; set; } = "«";
    public string PreviousPageText { get; set; } = "‹ Previous";
    public string SkipBackText { get; set; } = "..";
    public string SkipForwardText { get; set; } = "..";
    public string NextPageText { get; set; } = "Next ›";
    public string NextPageAriaLabel { get; set; } = "go to next page";
    
    public string PageSizeString { get; set; } = "age size:";
    
    public string LastPageText { get; set; } = "»";
    public bool FirstLastNavigation { get; set; } = true;
    public bool SkipForwardBackNavigation { get; set; } = true;
    
    // Optional htmx integration:
    // If set (e.g. "#content"), pagination links will include htmx attributes.
    public string HtmxTarget { get; set; } = "";
    
    public List<int> AvailablePageSizes { get; set; } = new() { 10, 25, 50, 100 };

    public int TotalPages => (int)Math.Ceiling((double)TotalItems! / (double)PageSize!);
    
    public int StartPage => Math.Max(1, Page - 2);
    public int EndPage => Math.Min(TotalPages, Page + 2);

    public string GetPageUrl(int page)
    {
        return $"{LinkUrl}?page={page}&pageSize={PageSize}";
    }
    
    public List<int> PageSizes => CalculatePageSizes();

    private List<int> CalculatePageSizes()
    {
        List<int> pageSizes = new();
        if (ShowPageSize)
        {
            int[] fixedSteps = { 10, 25, 50, 75, 100, 125, 150, 200, 250, 500, 1000 };

            foreach (var step in fixedSteps)
            {
                if (step <= TotalItems)
                {
                    pageSizes.Add(step);
                }
            }

            if (TotalItems > fixedSteps.Last())
            {
                int next = fixedSteps.Last();
                while (next < TotalItems)
                {
                    next *= 2;
                    if (next < TotalItems)
                    {
                        pageSizes.Add(next);
                    }
                }

                if (!pageSizes.Contains(TotalItems))
                {
                    pageSizes.Add(TotalItems);
                }
            }
        }
        return pageSizes; 
    }
}