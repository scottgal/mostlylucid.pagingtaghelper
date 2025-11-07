namespace mostlylucid.pagingtaghelper.sample.Models;

using mostlylucid.pagingtaghelper.Models;

public class SearchPagingViewModel : PagingViewModel, IPagingSearchModel
{
    public string? SearchTerm { get; set; }
}