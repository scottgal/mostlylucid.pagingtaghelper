namespace mostlylucid.pagingtaghelper.Models;

public interface IPagingSearchModel : IPagingModel
{
    public string? SearchTerm { get; set; }
}