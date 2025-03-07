namespace mostlylucid.pagingtaghelper.Models;

public abstract class BasePagerSearchMdodel : BasePagerModel, IPagingSearchModel
{
    public string? SearchTerm { get; set; }
}