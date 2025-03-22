using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using mostlylucig.pagingtaghelper.sample.Services;

namespace mostlylucig.pagingtaghelper.sample.Controllers;

public class SourceLinkController(GitHubCodeService gitHubCodeService, IMemoryCache memoryCache) : Controller
{
    // GET
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