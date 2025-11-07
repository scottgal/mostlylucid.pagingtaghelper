using System.Globalization;

namespace mostlylucid.pagingtaghelper.Services;

/// <summary>
/// Provides localized strings for paging components
/// </summary>
public interface IPagingLocalizer
{
    string FirstPageText { get; }
    string PreviousPageText { get; }
    string NextPageText { get; }
    string LastPageText { get; }
    string SkipBackText { get; }
    string SkipForwardText { get; }
    string NextPageAriaLabel { get; }
    string PreviousPageAriaLabel { get; }
    string PageSizeLabel { get; }
    string PageSizeString { get; }
    string GetPageSummary(int currentPage, int totalPages, int totalItems);
    void SetCulture(CultureInfo culture);
}
