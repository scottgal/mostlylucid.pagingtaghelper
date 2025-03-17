namespace mostlylucid.pagingtaghelper.Models;

public interface IOrderableBasePagerModel : IPagingModel
{
    string OrderBy { get; set; }
    bool Descending { get; set; }
}

public class OrderableBasePagerModel : BasePagerSearchMdodel, IOrderableBasePagerModel
{
    public string OrderBy { get; set; }
    
    public bool Descending { get; set; }
}