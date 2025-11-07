using mostlylucid.pagingtaghelper.Models;

namespace mostlylucid.pagingtaghelper.sample.Models;

public class PagingViewModel : BasePagerModel
{
    public List<FakeDataModel> Data { get; set; } = new List<FakeDataModel>();
}