using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using mostlylucid.pagingtaghelper.Components;

namespace mostlylucid.pagingtaghelper.tests.Components;

public class SortableHeaderTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithColumn_SetsAnchorTag()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("a");
    }

    [Fact]
    public async Task ProcessAsync_WithColumn_AddsOrderByToHref()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var href = output.Attributes["href"]?.Value?.ToString();
        href.Should().Contain("orderBy=Name");
    }

    [Fact]
    public async Task ProcessAsync_WhenNotSorted_ShowsUnsortedIcon()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.CurrentOrderBy = "Price"; // Different column
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        content.Should().Contain("bx-sort-alt-2");
    }

    [Fact]
    public async Task ProcessAsync_WhenSortedAscending_ShowsUpChevron()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.CurrentOrderBy = "Name";
        tagHelper.Descending = false;
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        content.Should().Contain("bx-chevron-up");
    }

    [Fact]
    public async Task ProcessAsync_WhenSortedDescending_ShowsDownChevron()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.CurrentOrderBy = "Name";
        tagHelper.Descending = true;
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        content.Should().Contain("bx-chevron-down");
    }

    [Fact]
    public async Task ProcessAsync_WhenSortedAscending_TogglesToDescending()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.CurrentOrderBy = "Name";
        tagHelper.Descending = false;
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var href = output.Attributes["href"]?.Value?.ToString();
        href.Should().Contain("descending=true");
    }

    [Fact]
    public async Task ProcessAsync_WhenSortedDescending_TogglesToAscending()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.CurrentOrderBy = "Name";
        tagHelper.Descending = true;
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var href = output.Attributes["href"]?.Value?.ToString();
        href.Should().Contain("descending=false");
    }

    [Fact]
    public async Task ProcessAsync_WithCustomChevronClasses_UsesCustomClasses()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.CurrentOrderBy = "Name";
        tagHelper.Descending = false;
        tagHelper.ChevronUpClass = "custom-up";
        tagHelper.ChevronDownClass = "custom-down";
        tagHelper.ChevronUnsortedClass = "custom-unsorted";
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        content.Should().Contain("custom-up");
    }

    [Fact]
    public async Task ProcessAsync_WithAutoAppendTrue_PreservesExistingQueryParameters()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper("?category=electronics&brand=sony");
        tagHelper.Column = "Name";
        tagHelper.AutoAppend = true;
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var href = output.Attributes["href"]?.Value?.ToString();
        href.Should().Contain("category=electronics");
        href.Should().Contain("brand=sony");
    }

    [Fact]
    public async Task ProcessAsync_WithAutoAppendFalse_DoesNotPreserveQueryParameters()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper("?category=electronics");
        tagHelper.Column = "Name";
        tagHelper.AutoAppend = false;
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var href = output.Attributes["href"]?.Value?.ToString();
        href.Should().NotContain("category=electronics");
    }

    [Fact]
    public async Task ProcessAsync_WithUseHtmx_AddsHxValsAttribute()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.UseHtmx = true;
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.Attributes.ContainsName("hx-vals").Should().BeTrue();
        var hxVals = output.Attributes["hx-vals"]?.Value?.ToString();
        hxVals.Should().Contain("Name");
    }

    [Fact]
    public async Task ProcessAsync_WithChildContent_UsesChildContentAsLabel()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var childContent = new DefaultTagHelperContent();
        childContent.SetContent("Product Name");

        var output = new TagHelperOutput(
            "sortable-header",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(childContent));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        content.Should().Contain("Product Name");
    }

    [Fact]
    public async Task ProcessAsync_WithoutChildContent_UsesColumnAsLabel()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        content.Should().Contain("Name");
    }

    [Fact]
    public async Task ProcessAsync_WithTagClass_AppliesCustomClass()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.TagClass = "custom-class";
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var classAttr = output.Attributes["class"]?.Value?.ToString();
        classAttr.Should().Contain("custom-class");
    }

    [Fact]
    public async Task ProcessAsync_RemovesTagHelperAttributes()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.Controller = "Products";
        tagHelper.Action = "Index";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();
        output.Attributes.SetAttribute("column", "Name");
        output.Attributes.SetAttribute("current-order-by", "Name");

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.Attributes.ContainsName("column").Should().BeFalse();
        output.Attributes.ContainsName("current-order-by").Should().BeFalse();
    }

    [Fact]
    public async Task ProcessAsync_WithHXActionAndController_UsesHashHref()
    {
        // Arrange
        var tagHelper = CreateSortableHeaderTagHelper();
        tagHelper.Column = "Name";
        tagHelper.HXAction = "Index";
        tagHelper.HXController = "Products";

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var href = output.Attributes["href"]?.Value?.ToString();
        href.Should().Be("#");
    }

    private static SortableHeaderTagHelper CreateSortableHeaderTagHelper(string? queryString = null)
    {
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
            .Returns("/products/index");

        var mockUrlHelperFactory = new Mock<IUrlHelperFactory>();
        mockUrlHelperFactory.Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>()))
            .Returns(mockUrlHelper.Object);

        var httpContext = new DefaultHttpContext();
        if (!string.IsNullOrEmpty(queryString))
        {
            httpContext.Request.QueryString = new QueryString(queryString);
        }

        var viewContext = new ViewContext
        {
            HttpContext = httpContext
        };

        return new SortableHeaderTagHelper(mockUrlHelperFactory.Object)
        {
            ViewContext = viewContext
        };
    }

    private static TagHelperContext CreateTagHelperContext()
    {
        return new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
    }

    private static TagHelperOutput CreateTagHelperOutput()
    {
        return new TagHelperOutput(
            "sortable-header",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
}
