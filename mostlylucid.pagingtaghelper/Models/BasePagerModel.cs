namespace mostlylucid.pagingtaghelper.Models;

public abstract class BasePagerModel : IPagingModel
{
    public int Page { get; set; } = 1;
    public int TotalItems { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public ViewType ViewType { get; set; } = ViewType.TailwindAndDaisy;

    public string LinkUrl { get; set; } = "";

}