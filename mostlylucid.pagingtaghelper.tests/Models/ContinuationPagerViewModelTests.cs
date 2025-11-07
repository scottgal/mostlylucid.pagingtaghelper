using FluentAssertions;
using mostlylucid.pagingtaghelper.Models;
using mostlylucid.pagingtaghelper.Models.TagModels;

namespace mostlylucid.pagingtaghelper.tests.Models;

public class ContinuationPagerViewModelTests
{
    [Fact]
    public void GetPreviousPageToken_ReturnsNull_WhenOnFirstPage()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            CurrentPage = 1,
            PageTokenHistory = new Dictionary<int, string>
            {
                { 1, "token1" },
                { 2, "token2" }
            }
        };

        // Act
        var result = model.GetPreviousPageToken();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetPreviousPageToken_ReturnsNull_WhenNoHistory()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            CurrentPage = 2,
            PageTokenHistory = null
        };

        // Act
        var result = model.GetPreviousPageToken();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetPreviousPageToken_ReturnsToken_WhenHistoryExists()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            CurrentPage = 3,
            PageTokenHistory = new Dictionary<int, string>
            {
                { 1, "token1" },
                { 2, "token2" }
            }
        };

        // Act
        var result = model.GetPreviousPageToken();

        // Assert
        result.Should().Be("token2");
    }

    [Fact]
    public void GetPreviousPageToken_ReturnsNull_WhenTokenNotInHistory()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            CurrentPage = 4,
            PageTokenHistory = new Dictionary<int, string>
            {
                { 1, "token1" },
                { 2, "token2" }
            }
        };

        // Act
        var result = model.GetPreviousPageToken();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CanNavigatePrevious_ReturnsFalse_WhenOnFirstPage()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            CurrentPage = 1,
            PageTokenHistory = new Dictionary<int, string>
            {
                { 1, "token1" }
            }
        };

        // Act
        var result = model.CanNavigatePrevious;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigatePrevious_ReturnsFalse_WhenNoPreviousToken()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            CurrentPage = 2,
            PageTokenHistory = null
        };

        // Act
        var result = model.CanNavigatePrevious;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigatePrevious_ReturnsTrue_WhenPreviousTokenExists()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            CurrentPage = 2,
            PageTokenHistory = new Dictionary<int, string>
            {
                { 1, "token1" }
            }
        };

        // Act
        var result = model.CanNavigatePrevious;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanNavigateNext_ReturnsFalse_WhenNoMoreResults()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            HasMoreResults = false,
            NextPageToken = "token123"
        };

        // Act
        var result = model.CanNavigateNext;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateNext_ReturnsFalse_WhenNoNextToken()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            HasMoreResults = true,
            NextPageToken = null
        };

        // Act
        var result = model.CanNavigateNext;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateNext_ReturnsFalse_WhenTokenIsEmpty()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            HasMoreResults = true,
            NextPageToken = ""
        };

        // Act
        var result = model.CanNavigateNext;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateNext_ReturnsTrue_WhenHasMoreResultsAndToken()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            HasMoreResults = true,
            NextPageToken = "token123"
        };

        // Act
        var result = model.CanNavigateNext;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void DefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var model = new ContinuationPagerViewModel();

        // Assert
        model.CurrentPage.Should().Be(1);
        model.PreviousPageText.Should().Be("‹ Previous");
        model.NextPageText.Should().Be("Next ›");
        model.PreviousPageAriaLabel.Should().Be("Go to previous page");
        model.NextPageAriaLabel.Should().Be("Go to next page");
        model.CssClass.Should().Be("flex gap-2 items-center");
        model.HtmxTarget.Should().Be(string.Empty);
        model.ShowPageSize.Should().BeTrue();
        model.ShowPageNumber.Should().BeTrue();
        model.EnableTokenAccumulation.Should().BeTrue();
    }

    [Fact]
    public void PageTokenHistory_CanBeModified()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            PageTokenHistory = new Dictionary<int, string>()
        };

        // Act
        model.PageTokenHistory[1] = "token1";
        model.PageTokenHistory[2] = "token2";
        model.PageTokenHistory[3] = "token3";

        // Assert
        model.PageTokenHistory.Should().HaveCount(3);
        model.PageTokenHistory[1].Should().Be("token1");
        model.PageTokenHistory[2].Should().Be("token2");
        model.PageTokenHistory[3].Should().Be("token3");
    }

    [Fact]
    public void Model_Property_CanStoreIContinuationPagingModel()
    {
        // Arrange
        var continuationModel = new TestContinuationModel
        {
            NextPageToken = "test-token",
            HasMoreResults = true,
            PageSize = 50,
            CurrentPage = 2
        };

        var model = new ContinuationPagerViewModel
        {
            Model = continuationModel
        };

        // Act & Assert
        model.Model.Should().NotBeNull();
        model.Model.Should().Be(continuationModel);
        model.Model.NextPageToken.Should().Be("test-token");
        model.Model.HasMoreResults.Should().BeTrue();
    }

    [Fact]
    public void ShowPageSize_CanBeDisabled()
    {
        // Arrange & Act
        var model = new ContinuationPagerViewModel
        {
            ShowPageSize = false
        };

        // Assert
        model.ShowPageSize.Should().BeFalse();
    }

    [Fact]
    public void ShowPageNumber_CanBeDisabled()
    {
        // Arrange & Act
        var model = new ContinuationPagerViewModel
        {
            ShowPageNumber = false
        };

        // Assert
        model.ShowPageNumber.Should().BeFalse();
    }

    [Fact]
    public void EnableTokenAccumulation_CanBeDisabled()
    {
        // Arrange & Act
        var model = new ContinuationPagerViewModel
        {
            EnableTokenAccumulation = false
        };

        // Assert
        model.EnableTokenAccumulation.Should().BeFalse();
    }

    [Fact]
    public void CustomText_CanBeSet()
    {
        // Arrange & Act
        var model = new ContinuationPagerViewModel
        {
            PreviousPageText = "Back",
            NextPageText = "Forward",
            PreviousPageAriaLabel = "Go back",
            NextPageAriaLabel = "Go forward"
        };

        // Assert
        model.PreviousPageText.Should().Be("Back");
        model.NextPageText.Should().Be("Forward");
        model.PreviousPageAriaLabel.Should().Be("Go back");
        model.NextPageAriaLabel.Should().Be("Go forward");
    }

    [Fact]
    public void HtmxTarget_CanBeSet()
    {
        // Arrange & Act
        var model = new ContinuationPagerViewModel
        {
            HtmxTarget = "#results-container"
        };

        // Assert
        model.HtmxTarget.Should().Be("#results-container");
    }

    // Helper class for testing
    private class TestContinuationModel : IContinuationPagingModel
    {
        public string? NextPageToken { get; set; }
        public bool HasMoreResults { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public ViewType ViewType { get; set; }
        public Dictionary<int, string>? PageTokenHistory { get; set; }
    }
}
