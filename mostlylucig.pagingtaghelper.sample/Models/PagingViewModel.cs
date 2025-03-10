using mostlylucid.pagingtaghelper.Models;

namespace mostlylucig.pagingtaghelper.sample.Models;

public class PagingViewModel : BasePagerModel
{
    public List<FakeDataModel> Data { get; set; } = new List<FakeDataModel>();
}