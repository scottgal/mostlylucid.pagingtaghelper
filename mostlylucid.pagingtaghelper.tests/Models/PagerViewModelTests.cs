using FluentAssertions;
using Moq;
using mostlylucid.pagingtaghelper.Models.TagModels;
using mostlylucid.pagingtaghelper.Services;

namespace mostlylucid.pagingtaghelper.tests.Models;

public class PagerViewModelTests
{
    [Fact]
    public void TotalPages_CalculatesCorrectly()
    {
        // Arrange
        var model = new PagerViewModel
        {
            TotalItems = 100,
            PageSize = 10
        };

        // Act
        var result = model.TotalPages;

        // Assert
        result.Should().Be(10);
    }

    [Fact]
    public void TotalPages_RoundsUpCorrectly()
    {
        // Arrange
        var model = new PagerViewModel
        {
            TotalItems = 105,
            PageSize = 10
        };

        // Act
        var result = model.TotalPages;

        // Assert
        result.Should().Be(11);
    }

    [Fact]
    public void StartPage_CalculatesCorrectly_WhenPageIsLow()
    {
        // Arrange
        var model = new PagerViewModel
        {
            Page = 1,
            TotalItems = 100,
            PageSize = 10
        };

        // Act
        var result = model.StartPage;

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void EndPage_CalculatesCorrectly_WhenPageIsHigh()
    {
        // Arrange
        var model = new PagerViewModel
        {
            Page = 10,
            TotalItems = 100,
            PageSize = 10,
            PagesToDisplay = 5
        };

        // Act
        var result = model.EndPage;

        // Assert
        result.Should().Be(10);
    }

    [Fact]
    public void FirstPageText_WithLocalizer_ReturnsLocalizedValue()
    {
        // Arrange
        var mockLocalizer = new Mock<IPagingLocalizer>();
        mockLocalizer.Setup(l => l.FirstPageText).Returns("First");

        var model = new PagerViewModel
        {
            Localizer = mockLocalizer.Object
        };

        // Act
        var result = model.FirstPageText;

        // Assert
        result.Should().Be("First");
    }

    [Fact]
    public void FirstPageText_WithoutLocalizer_ReturnsDefault()
    {
        // Arrange
        var model = new PagerViewModel();

        // Act
        var result = model.FirstPageText;

        // Assert
        result.Should().Be("«");
    }

    [Fact]
    public void FirstPageText_WithExplicitValue_ReturnsExplicitValue()
    {
        // Arrange
        var mockLocalizer = new Mock<IPagingLocalizer>();
        mockLocalizer.Setup(l => l.FirstPageText).Returns("First");

        var model = new PagerViewModel
        {
            Localizer = mockLocalizer.Object,
            FirstPageText = "Custom"
        };

        // Act
        var result = model.FirstPageText;

        // Assert
        result.Should().Be("Custom");
    }

    [Fact]
    public void PreviousPageText_WithLocalizer_ReturnsLocalizedValue()
    {
        // Arrange
        var mockLocalizer = new Mock<IPagingLocalizer>();
        mockLocalizer.Setup(l => l.PreviousPageText).Returns("Anterior");

        var model = new PagerViewModel
        {
            Localizer = mockLocalizer.Object
        };

        // Act
        var result = model.PreviousPageText;

        // Assert
        result.Should().Be("Anterior");
    }

    [Fact]
    public void NextPageText_WithLocalizer_ReturnsLocalizedValue()
    {
        // Arrange
        var mockLocalizer = new Mock<IPagingLocalizer>();
        mockLocalizer.Setup(l => l.NextPageText).Returns("Siguiente");

        var model = new PagerViewModel
        {
            Localizer = mockLocalizer.Object
        };

        // Act
        var result = model.NextPageText;

        // Assert
        result.Should().Be("Siguiente");
    }

    [Fact]
    public void LastPageText_WithLocalizer_ReturnsLocalizedValue()
    {
        // Arrange
        var mockLocalizer = new Mock<IPagingLocalizer>();
        mockLocalizer.Setup(l => l.LastPageText).Returns("Last");

        var model = new PagerViewModel
        {
            Localizer = mockLocalizer.Object
        };

        // Act
        var result = model.LastPageText;

        // Assert
        result.Should().Be("Last");
    }

    [Fact]
    public void SkipBackText_WithLocalizer_ReturnsLocalizedValue()
    {
        // Arrange
        var mockLocalizer = new Mock<IPagingLocalizer>();
        mockLocalizer.Setup(l => l.SkipBackText).Returns("...");

        var model = new PagerViewModel
        {
            Localizer = mockLocalizer.Object
        };

        // Act
        var result = model.SkipBackText;

        // Assert
        result.Should().Be("...");
    }

    [Fact]
    public void SkipForwardText_WithLocalizer_ReturnsLocalizedValue()
    {
        // Arrange
        var mockLocalizer = new Mock<IPagingLocalizer>();
        mockLocalizer.Setup(l => l.SkipForwardText).Returns("...");

        var model = new PagerViewModel
        {
            Localizer = mockLocalizer.Object
        };

        // Act
        var result = model.SkipForwardText;

        // Assert
        result.Should().Be("...");
    }

    [Fact]
    public void NextPageAriaLabel_WithLocalizer_ReturnsLocalizedValue()
    {
        // Arrange
        var mockLocalizer = new Mock<IPagingLocalizer>();
        mockLocalizer.Setup(l => l.NextPageAriaLabel).Returns("go to next");

        var model = new PagerViewModel
        {
            Localizer = mockLocalizer.Object
        };

        // Act
        var result = model.NextPageAriaLabel;

        // Assert
        result.Should().Be("go to next");
    }

    [Fact]
    public void PageSizeString_WithLocalizer_ReturnsLocalizedValue()
    {
        // Arrange
        var mockLocalizer = new Mock<IPagingLocalizer>();
        mockLocalizer.Setup(l => l.PageSizeLabel).Returns("Tamaño:");

        var model = new PagerViewModel
        {
            Localizer = mockLocalizer.Object
        };

        // Act
        var result = model.PageSizeString;

        // Assert
        result.Should().Be("Tamaño:");
    }

    [Fact]
    public void GetPageSummary_WithLocalizer_ReturnsLocalizedSummary()
    {
        // Arrange
        var mockLocalizer = new Mock<IPagingLocalizer>();
        mockLocalizer.Setup(l => l.GetPageSummary(1, 10, 100))
            .Returns("Página 1 de 10 (Total: 100)");

        var model = new PagerViewModel
        {
            Page = 1,
            TotalItems = 100,
            PageSize = 10,
            Localizer = mockLocalizer.Object
        };

        // Act
        var result = model.GetPageSummary();

        // Assert
        result.Should().Be("Página 1 de 10 (Total: 100)");
    }

    [Fact]
    public void GetPageSummary_WithoutLocalizer_ReturnsDefaultSummary()
    {
        // Arrange
        var model = new PagerViewModel
        {
            Page = 2,
            TotalItems = 50,
            PageSize = 10
        };

        // Act
        var result = model.GetPageSummary();

        // Assert
        result.Should().Be("Page 2 of 5 (Total items: 50)");
    }

    [Theory]
    [InlineData(1, 100, 10, 1, 5)]
    [InlineData(3, 100, 10, 1, 5)]
    [InlineData(10, 100, 10, 6, 10)]
    public void StartPage_EndPage_CalculateCorrectRange(
        int currentPage,
        int totalItems,
        int pageSize,
        int expectedStart,
        int expectedEnd)
    {
        // Arrange
        var model = new PagerViewModel
        {
            Page = currentPage,
            TotalItems = totalItems,
            PageSize = pageSize,
            PagesToDisplay = 5
        };

        // Act
        var startPage = model.StartPage;
        var endPage = model.EndPage;

        // Assert
        startPage.Should().Be(expectedStart);
        endPage.Should().Be(expectedEnd);
    }

    [Fact]
    public void ShowSummary_DefaultsToTrue()
    {
        // Arrange & Act
        var model = new PagerViewModel();

        // Assert
        model.ShowSummary.Should().BeTrue();
    }

    [Fact]
    public void ShowPageSize_DefaultsToTrue()
    {
        // Arrange & Act
        var model = new PagerViewModel();

        // Assert
        model.ShowPageSize.Should().BeTrue();
    }

    [Fact]
    public void FirstLastNavigation_DefaultsToTrue()
    {
        // Arrange & Act
        var model = new PagerViewModel();

        // Assert
        model.FirstLastNavigation.Should().BeTrue();
    }

    [Fact]
    public void SkipForwardBackNavigation_DefaultsToTrue()
    {
        // Arrange & Act
        var model = new PagerViewModel();

        // Assert
        model.SkipForwardBackNavigation.Should().BeTrue();
    }

    [Fact]
    public void PagesToDisplay_DefaultsToFive()
    {
        // Arrange & Act
        var model = new PagerViewModel();

        // Assert
        model.PagesToDisplay.Should().Be(5);
    }
}
