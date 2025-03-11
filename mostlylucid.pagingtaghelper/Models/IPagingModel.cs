namespace mostlylucid.pagingtaghelper.Models;

public interface IPagingModel
{
    public int Page { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }

    public ViewType ViewType { get; set; }
    
    public string LinkUrl { get; set; }
}