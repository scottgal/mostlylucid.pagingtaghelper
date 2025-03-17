using mostlylucid.pagingtaghelper.Models;

namespace mostlylucig.pagingtaghelper.sample.Models;

public class OrderedPagingViewModel : SearchPagingViewModel, IOrderableBasePagerModel
{
    public string OrderBy { get; set; }
    public bool Descending { get; set; }
}