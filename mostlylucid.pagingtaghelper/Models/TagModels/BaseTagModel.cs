namespace mostlylucid.pagingtaghelper.Models.TagModels;

public abstract class BaseTagModel
{

    public List<int> PageSizes { get; set; }


    public IPagingModel? Model { get; set; }

        
    public int MaxPageSize { get; set; } = 100;
    

    public ViewType ViewType { get; set; }
    public bool UseLocalView { get; set; } = false;
    public string? PagerId { get; set; }

    public string? SearchTerm { get; set; }
    public int PageSize { get; set; }

    public int TotalItems { get; set; }



}