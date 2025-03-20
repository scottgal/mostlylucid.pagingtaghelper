namespace mostlylucid.pagingtaghelper.Models.TagModels;

public abstract class BaseTagModel
{
    private List<int>? _oageSizes;

    public List<int> PageSizes
    {
        get { return _oageSizes ??= CalculatePageSizes(); }
    }

    public IPagingModel? Model { get; set; }

    public int TotalPages => (int)Math.Ceiling(TotalItems! / (double)PageSize!);
    public ViewType ViewType { get; set; }
    public bool UseLocalView { get; set; } = false;
    public string? PagerId { get; set; }

    public string? SearchTerm { get; set; }
    public int PageSize { get; set; }

    public int TotalItems { get; set; }
    public int Page { get; set; }


    private List<int> CalculatePageSizes()
    {
        List<int> pageSizes = new();

            int[] fixedSteps = { 10, 25, 50, 75, 100, 125, 150, 200, 250, 500, 1000 };

            foreach (var step in fixedSteps)
                if (step <= TotalItems)
                    pageSizes.Add(step);

            if (TotalItems > fixedSteps.Last())
            {
                var next = fixedSteps.Last();
                while (next < TotalItems)
                {
                    next *= 2;
                    if (next < TotalItems) pageSizes.Add(next);
                }

              
            }
            if (!pageSizes.Contains(TotalItems)) pageSizes.Add(TotalItems);
            
        

        return pageSizes;
    }
}