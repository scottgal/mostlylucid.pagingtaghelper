using mostlylucid.pagingtaghelper.Models;

namespace mostlylucid.pagingtaghelper.sample.Models;

public class OrderedPagingViewModel : SearchPagingViewModel, IOrderableBasePagerModel
{
    public string OrderBy { get; set; }
    public bool Descending { get; set; }
}