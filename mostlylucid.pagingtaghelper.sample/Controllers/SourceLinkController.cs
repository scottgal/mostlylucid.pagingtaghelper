using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using mostlylucid.pagingtaghelper.sample.Services;

namespace mostlylucid.pagingtaghelper.sample.Controllers;

public class SourceLinkController(GitHubCodeService gitHubCodeService, IMemoryCache memoryCache) : Controller
{
    // GET
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "sourceLink" }, VaryByHeader = "Accept-Encoding")]
    public async Task<IActionResult> ShowSource(string sourceLink)
    {
        var key = $"SourceLinkController_ShowSource_{sourceLink}";
        var source = await memoryCache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            var source = await gitHubCodeService.GetCodeFromGitHubAsync("scottgal", "mostlylucid.pagingtaghelper", "refs/heads/main/mostlylucid.pagingtaghelper.sample",
                sourceLink);
            return source;
           
        });
        return PartialView("_ShowSource", source);
    }
}