using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using mostlylucid.pagingtaghelper.Extensions;

namespace mostlylucid.pagingtaghelper.tests.Extensions;

public class QueryStringExtensionsTests
{
    [Fact]
    public void BuildPagedUrl_WithPageAndPageSize_ReturnsCorrectUrl()
    {
        // Arrange
        var context = CreateHttpContext();
        var baseUrl = "/products";

        // Act
        var result = context.BuildPagedUrl(baseUrl, 2, 25);

        // Assert
        result.Should().Contain("page=2");
        result.Should().Contain("pageSize=25");
    }

    [Fact]
    public void BuildPagedUrl_WithSearchTerm_IncludesSearchParameter()
    {
        // Arrange
        var context = CreateHttpContext();
        var baseUrl = "/products";

        // Act
        var result = context.BuildPagedUrl(baseUrl, 1, 10, searchTerm: "test");

        // Assert
        result.Should().Contain("search=test");
    }

    [Fact]
    public void BuildPagedUrl_WithOrderBy_IncludesOrderByParameter()
    {
        // Arrange
        var context = CreateHttpContext();
        var baseUrl = "/products";

        // Act
        var result = context.BuildPagedUrl(baseUrl, 1, 10, orderBy: "Name");

        // Assert
        result.Should().Contain("orderBy=Name");
    }

    [Fact]
    public void BuildPagedUrl_WithDescending_IncludesDescendingParameter()
    {
        // Arrange
        var context = CreateHttpContext();
        var baseUrl = "/products";

        // Act
        var result = context.BuildPagedUrl(baseUrl, 1, 10, descending: true);

        // Assert
        result.Should().Contain("descending=true");
    }

    [Fact]
    public void BuildPagedUrl_WithDescendingFalse_IncludesDescendingFalseParameter()
    {
        // Arrange
        var context = CreateHttpContext();
        var baseUrl = "/products";

        // Act
        var result = context.BuildPagedUrl(baseUrl, 1, 10, descending: false);

        // Assert
        result.Should().Contain("descending=false");
    }

    [Fact]
    public void BuildPagedUrl_WithExistingQueryString_PreservesOtherParameters()
    {
        // Arrange
        var context = CreateHttpContext("?category=electronics&brand=sony");
        var baseUrl = "/products";

        // Act
        var result = context.BuildPagedUrl(baseUrl, 1, 10);

        // Assert
        result.Should().Contain("category=electronics");
        result.Should().Contain("brand=sony");
    }

    [Fact]
    public void BuildPagedUrl_WithExcludeKeys_RemovesExcludedParameters()
    {
        // Arrange
        var context = CreateHttpContext("?category=electronics&brand=sony&oldParam=value");
        var baseUrl = "/products";

        // Act
        var result = context.BuildPagedUrl(baseUrl, 1, 10, excludeKeys: new[] { "oldParam" });

        // Assert
        result.Should().NotContain("oldParam");
        result.Should().Contain("category=electronics");
        result.Should().Contain("brand=sony");
    }

    [Fact]
    public void BuildPagedUrl_WithAllParameters_ReturnsCompleteUrl()
    {
        // Arrange
        var context = CreateHttpContext();
        var baseUrl = "/products";

        // Act
        var result = context.BuildPagedUrl(baseUrl, 3, 50, "laptop", "Price", true);

        // Assert
        result.Should().Contain("page=3");
        result.Should().Contain("pageSize=50");
        result.Should().Contain("search=laptop");
        result.Should().Contain("orderBy=Price");
        result.Should().Contain("descending=true");
    }

    [Fact]
    public void BuildPagedUrl_WithSpecialCharactersInSearchTerm_EscapesCorrectly()
    {
        // Arrange
        var context = CreateHttpContext();
        var baseUrl = "/products";

        // Act
        var result = context.BuildPagedUrl(baseUrl, 1, 10, searchTerm: "test & value");

        // Assert
        result.Should().Contain("search=test%20%26%20value");
    }

    [Fact]
    public void BuildPagedUrl_WithEmptySearchTerm_DoesNotIncludeSearchParameter()
    {
        // Arrange
        var context = CreateHttpContext();
        var baseUrl = "/products";

        // Act
        var result = context.BuildPagedUrl(baseUrl, 1, 10, searchTerm: "");

        // Assert
        result.Should().NotContain("search=");
    }

    [Fact]
    public void BuildPagedUrl_WithNullSearchTerm_DoesNotIncludeSearchParameter()
    {
        // Arrange
        var context = CreateHttpContext();
        var baseUrl = "/products";

        // Act
        var result = context.BuildPagedUrl(baseUrl, 1, 10, searchTerm: null);

        // Assert
        result.Should().NotContain("search=");
    }

    [Fact]
    public void BuildPagedUrl_OverridesExistingPageParameter()
    {
        // Arrange
        var context = CreateHttpContext("?page=5&pageSize=20");
        var baseUrl = "/products";

        // Act
        var result = context.BuildPagedUrl(baseUrl, 2, 10);

        // Assert
        result.Should().Contain("page=2");
        result.Should().Contain("pageSize=10");
        result.Should().NotContain("page=5");
        result.Should().NotContain("pageSize=20");
    }

    private static HttpContext CreateHttpContext(string? queryString = null)
    {
        var context = new DefaultHttpContext();
        if (!string.IsNullOrEmpty(queryString))
        {
            context.Request.QueryString = new QueryString(queryString);
        }
        return context;
    }
}
