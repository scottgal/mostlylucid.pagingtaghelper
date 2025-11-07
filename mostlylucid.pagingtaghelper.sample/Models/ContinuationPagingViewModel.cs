using mostlylucid.pagingtaghelper.Models;

namespace mostlylucig.pagingtaghelper.sample.Models;

public class ContinuationPagingViewModel : IContinuationPagingModel
{
    public string? NextPageToken { get; set; }
    public bool HasMoreResults { get; set; }
    public int PageSize { get; set; } = 25;
    public int CurrentPage { get; set; } = 1;
    public Dictionary<int, string>? PageTokenHistory { get; set; }
    public ViewType ViewType { get; set; } = ViewType.TailwindAndDaisy;

    public List<FakeDataModel> Products { get; set; } = new();
}
