using FluentAssertions;
using mostlylucid.pagingtaghelper.Models.TagModels;

namespace mostlylucid.pagingtaghelper.tests.Models;

public class ContinuationPagerViewModelParameterPrefixTests
{
    [Fact]
    public void GetParameterName_ReturnsBaseName_WhenNoPrefixSet()
    {
        // Arrange
        var model = new ContinuationPagerViewModel();

        // Act
        var result = model.GetParameterName("pageToken");

        // Assert
        result.Should().Be("pageToken");
    }

    [Fact]
    public void GetParameterName_ReturnsPrefixedName_WhenPrefixSet()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            ParameterPrefix = "products"
        };

        // Act
        var result = model.GetParameterName("pageToken");

        // Assert
        result.Should().Be("products_pageToken");
    }

    [Theory]
    [InlineData("pageToken", "products_pageToken")]
    [InlineData("currentPage", "products_currentPage")]
    [InlineData("pageSize", "products_pageSize")]
    [InlineData("tokenHistory", "products_tokenHistory")]
    public void GetParameterName_AppliesPrefixCorrectly(string baseName, string expected)
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            ParameterPrefix = "products"
        };

        // Act
        var result = model.GetParameterName(baseName);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetParameterName_HandlesEmptyPrefix()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            ParameterPrefix = ""
        };

        // Act
        var result = model.GetParameterName("pageToken");

        // Assert
        result.Should().Be("pageToken");
    }

    [Fact]
    public void GetParameterName_HandlesWhitespacePrefix()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            ParameterPrefix = "   "
        };

        // Act
        var result = model.GetParameterName("pageToken");

        // Assert
        result.Should().Be("pageToken");
    }

    [Fact]
    public void MultiplePagersScenario_WithDifferentPrefixes()
    {
        // Arrange
        var productsPager = new ContinuationPagerViewModel
        {
            ParameterPrefix = "products",
            CurrentPage = 1,
            PageSize = 25,
            NextPageToken = "products-token-123",
            HasMoreResults = true
        };

        var customersPager = new ContinuationPagerViewModel
        {
            ParameterPrefix = "customers",
            CurrentPage = 2,
            PageSize = 10,
            NextPageToken = "customers-token-456",
            HasMoreResults = true
        };

        // Act & Assert
        productsPager.GetParameterName("pageToken").Should().Be("products_pageToken");
        productsPager.GetParameterName("currentPage").Should().Be("products_currentPage");

        customersPager.GetParameterName("pageToken").Should().Be("customers_pageToken");
        customersPager.GetParameterName("currentPage").Should().Be("customers_currentPage");

        // Verify they don't interfere with each other
        productsPager.NextPageToken.Should().Be("products-token-123");
        customersPager.NextPageToken.Should().Be("customers-token-456");
    }

    [Fact]
    public void ParameterPrefix_CanBeSetAfterConstruction()
    {
        // Arrange
        var model = new ContinuationPagerViewModel();
        model.GetParameterName("pageToken").Should().Be("pageToken");

        // Act
        model.ParameterPrefix = "orders";

        // Assert
        model.GetParameterName("pageToken").Should().Be("orders_pageToken");
    }

    [Fact]
    public void ParameterPrefix_CanBeCleared()
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            ParameterPrefix = "products"
        };
        model.GetParameterName("pageToken").Should().Be("products_pageToken");

        // Act
        model.ParameterPrefix = null;

        // Assert
        model.GetParameterName("pageToken").Should().Be("pageToken");
    }

    [Theory]
    [InlineData("my-prefix")]
    [InlineData("MyPrefix")]
    [InlineData("prefix123")]
    [InlineData("PrEfIx")]
    public void GetParameterName_PreservesCase(string prefix)
    {
        // Arrange
        var model = new ContinuationPagerViewModel
        {
            ParameterPrefix = prefix
        };

        // Act
        var result = model.GetParameterName("pageToken");

        // Assert
        result.Should().Be($"{prefix}_pageToken");
    }

    [Fact]
    public void DefaultParameterPrefix_IsNull()
    {
        // Arrange & Act
        var model = new ContinuationPagerViewModel();

        // Assert
        model.ParameterPrefix.Should().BeNull();
    }
}
