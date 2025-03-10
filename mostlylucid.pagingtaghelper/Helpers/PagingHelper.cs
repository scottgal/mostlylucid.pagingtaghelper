using Microsoft.AspNetCore.Http;

namespace mostlylucid.pagingtaghelper.Helpers;

public static class PagingHelper
{
    private const string PageRequestHeader = "pagerequest";
    public static bool IsPageRequest(this HttpRequest request)
    {
        if(!request.Headers.ContainsKey(PageRequestHeader)) return false;
        var requestHeader = request.Headers[PageRequestHeader];
        return requestHeader == "true";
    }
}