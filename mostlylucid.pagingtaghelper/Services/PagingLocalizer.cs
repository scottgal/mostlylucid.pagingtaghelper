using System.Globalization;
using mostlylucid.pagingtaghelper.Resources;

namespace mostlylucid.pagingtaghelper.Services;

/// <summary>
/// Default implementation of IPagingLocalizer using resource files
/// </summary>
public class PagingLocalizer : IPagingLocalizer
{
    private CultureInfo? _culture;

    public string FirstPageText => GetString(PagingResources.ResourceManager, nameof(PagingResources.FirstPageText)) ?? "«";

    public string PreviousPageText => GetString(PagingResources.ResourceManager, nameof(PagingResources.PreviousPageText)) ?? "‹ Previous";

    public string NextPageText => GetString(PagingResources.ResourceManager, nameof(PagingResources.NextPageText)) ?? "Next ›";

    public string LastPageText => GetString(PagingResources.ResourceManager, nameof(PagingResources.LastPageText)) ?? "»";

    public string SkipBackText => GetString(PagingResources.ResourceManager, nameof(PagingResources.SkipBackText)) ?? "..";

    public string SkipForwardText => GetString(PagingResources.ResourceManager, nameof(PagingResources.SkipForwardText)) ?? "..";

    public string NextPageAriaLabel => GetString(PagingResources.ResourceManager, nameof(PagingResources.NextPageAriaLabel)) ?? "go to next page";

    public string PageSizeLabel => GetString(PagingResources.ResourceManager, nameof(PagingResources.PageSizeLabel)) ?? "Page size:";

    public string GetPageSummary(int currentPage, int totalPages, int totalItems)
    {
        var format = GetString(PagingResources.ResourceManager, nameof(PagingResources.PageSummaryFormat))
                     ?? "Page {0} of {1} (Total items: {2})";
        return string.Format(format, currentPage, totalPages, totalItems);
    }

    public void SetCulture(CultureInfo culture)
    {
        _culture = culture;
    }

    private string? GetString(System.Resources.ResourceManager resourceManager, string name)
    {
        return resourceManager.GetString(name, _culture ?? CultureInfo.CurrentUICulture);
    }
}
