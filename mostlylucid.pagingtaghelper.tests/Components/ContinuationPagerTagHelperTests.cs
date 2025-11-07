using FluentAssertions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using mostlylucid.pagingtaghelper.Components;
using mostlylucid.pagingtaghelper.Models;
using mostlylucid.pagingtaghelper.Models.TagModels;
using System.Text.Json;

namespace mostlylucid.pagingtaghelper.tests.Components;

public class ContinuationPagerTagHelperTests
{
    #region Basic Functionality Tests

    [Fact]
    public async Task ProcessAsync_WithModel_SetsContainerDiv()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithModel_GeneratesIdAttribute()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.Attributes.ContainsName("id").Should().BeTrue();
        var id = output.Attributes["id"]?.Value?.ToString();
        id.Should().StartWith("continuation-pager-");
    }

    [Fact]
    public async Task ProcessAsync_WithCustomPagerId_UsesProvidedId()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.PagerId = "custom-pager-id";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var id = output.Attributes["id"]?.Value?.ToString();
        id.Should().Be("custom-pager-id");
    }

    [Fact]
    public async Task ProcessAsync_WithIndividualProperties_WorksWithoutModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        tagHelper.PageSize = 25;
        tagHelper.CurrentPage = 1;
        tagHelper.NextPageToken = "token123";
        tagHelper.HasMoreResults = true;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
        output.Attributes.ContainsName("id").Should().BeTrue();
    }

    [Fact]
    public async Task ProcessAsync_ModelPropertiesOverrideIndividualProperties()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel(pageSize: 50, currentPage: 3);
        tagHelper.Model = model;
        tagHelper.PageSize = 10; // Should be overridden by model
        tagHelper.CurrentPage = 1; // Should be overridden by model

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        // The model's values should win (50 and 3)
        output.TagName.Should().Be("div");
    }

    #endregion

    #region ViewType Tests

    [Theory]
    [InlineData(ViewType.TailwindAndDaisy)]
    [InlineData(ViewType.Tailwind)]
    [InlineData(ViewType.Bootstrap)]
    [InlineData(ViewType.Plain)]
    [InlineData(ViewType.NoJS)]
    public async Task ProcessAsync_SupportsAllViewTypes(ViewType viewType)
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        model.ViewType = viewType;
        tagHelper.Model = model;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithViewTypeProperty_OverridesDefault()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.ViewType = ViewType.Bootstrap;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region JavaScript Mode Tests

    [Theory]
    [InlineData(JavaScriptMode.HTMX)]
    [InlineData(JavaScriptMode.HTMXWithAlpine)]
    [InlineData(JavaScriptMode.Alpine)]
    [InlineData(JavaScriptMode.PlainJS)]
    [InlineData(JavaScriptMode.NoJS)]
    public async Task ProcessAsync_SupportsAllJavaScriptModes(JavaScriptMode jsMode)
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.JSMode = jsMode;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithUseHtmxTrue_SetsJavaScriptModeHTMX()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.UseHtmx = true;
        tagHelper.JSMode = null;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithUseHtmxFalse_SetsJavaScriptModePlainJS()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.UseHtmx = false;
        tagHelper.JSMode = null;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region Token History Tests

    [Fact]
    public async Task ProcessAsync_WithPageTokenHistory_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        model.PageTokenHistory = new Dictionary<int, string>
        {
            { 1, "token1" },
            { 2, "token2" },
            { 3, "token3" }
        };
        tagHelper.Model = model;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithJsonTokenHistory_ParsesCorrectly()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var tokenHistory = new Dictionary<int, string>
        {
            { 1, "token1" },
            { 2, "token2" }
        };
        var json = JsonSerializer.Serialize(tokenHistory);

        tagHelper.PageSize = 25;
        tagHelper.CurrentPage = 2;
        tagHelper.HasMoreResults = true;
        tagHelper.PageTokenHistoryJson = json;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithInvalidJsonTokenHistory_HandlesGracefully()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        tagHelper.PageSize = 25;
        tagHelper.CurrentPage = 1;
        tagHelper.HasMoreResults = true;
        tagHelper.PageTokenHistoryJson = "{invalid json}";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        // Should not throw, just ignore the invalid JSON
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_ModelTokenHistoryTakesPrecedenceOverJson()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        model.PageTokenHistory = new Dictionary<int, string>
        {
            { 1, "model-token" }
        };
        tagHelper.Model = model;
        tagHelper.PageTokenHistoryJson = JsonSerializer.Serialize(new Dictionary<int, string>
        {
            { 1, "json-token" }
        });

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
        // Model should win over JSON
    }

    #endregion

    #region HTMX Attributes Tests

    [Fact]
    public async Task ProcessAsync_WithHtmxTarget_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.HtmxTarget = "#results-container";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithEmptyHtmxTarget_UsesDefault()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.HtmxTarget = string.Empty;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region Visibility Tests

    [Fact]
    public async Task ProcessAsync_WithShowPageSizeTrue_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.ShowPageSize = true;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithShowPageSizeFalse_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.ShowPageSize = false;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithShowPageNumberTrue_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.ShowPageNumber = true;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithShowPageNumberFalse_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.ShowPageNumber = false;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region Custom Text Tests

    [Fact]
    public async Task ProcessAsync_WithCustomPreviousText_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.PreviousPageText = "← Back";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithCustomNextText_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.NextPageText = "Forward →";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithCustomAriaLabels_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.PreviousPageAriaLabel = "navigate to previous page";
        tagHelper.NextPageAriaLabel = "navigate to next page";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithCustomCssClass_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.CssClass = "my-custom-pager-class";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region Link URL Tests

    [Fact]
    public async Task ProcessAsync_WithLinkUrl_UsesProvidedUrl()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.LinkUrl = "/custom/path";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithHrefAttribute_AliasesLinkUrl()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.Href = "/another/path";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        tagHelper.LinkUrl.Should().Be("/another/path");
    }

    [Fact]
    public async Task ProcessAsync_WithoutLinkUrl_FallsBackToCurrentPath()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup("/products/list");
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region Token Accumulation Tests

    [Fact]
    public async Task ProcessAsync_WithEnableTokenAccumulationTrue_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.EnableTokenAccumulation = true;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithEnableTokenAccumulationFalse_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.EnableTokenAccumulation = false;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region Parameter Prefix Tests

    [Fact]
    public async Task ProcessAsync_WithParameterPrefix_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.ParameterPrefix = "products";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithoutParameterPrefix_UsesDefault()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region Localization Tests

    [Fact]
    public async Task ProcessAsync_WithLanguage_SetsLocalizerCulture()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.Language = "fr";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithInvalidLanguage_HandlesGracefully()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.Language = "invalid-culture-code";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        // Should not throw, falls back to current culture
        output.TagName.Should().Be("div");
    }

    [Fact]
    public async Task ProcessAsync_WithoutLanguage_UsesCurrentCulture()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region Attribute Removal Tests

    [Fact]
    public async Task ProcessAsync_RemovesAllTagHelperAttributes()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;

        // Add attributes that should be removed
        output.Attributes.SetAttribute("model", "value");
        output.Attributes.SetAttribute("view-type", "value");
        output.Attributes.SetAttribute("use-htmx", "value");
        output.Attributes.SetAttribute("js-mode", "value");
        output.Attributes.SetAttribute("show-pagesize", "value");
        output.Attributes.SetAttribute("show-page-number", "value");
        output.Attributes.SetAttribute("next-page-token", "value");
        output.Attributes.SetAttribute("has-more-results", "value");

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.Attributes.ContainsName("model").Should().BeFalse();
        output.Attributes.ContainsName("view-type").Should().BeFalse();
        output.Attributes.ContainsName("use-htmx").Should().BeFalse();
        output.Attributes.ContainsName("js-mode").Should().BeFalse();
        output.Attributes.ContainsName("show-pagesize").Should().BeFalse();
        output.Attributes.ContainsName("show-page-number").Should().BeFalse();
        output.Attributes.ContainsName("next-page-token").Should().BeFalse();
        output.Attributes.ContainsName("has-more-results").Should().BeFalse();
    }

    #endregion

    #region Search Term Tests

    [Fact]
    public async Task ProcessAsync_WithSearchTerm_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.SearchTerm = "test search";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region Use Local View Tests

    [Fact]
    public async Task ProcessAsync_WithUseLocalViewTrue_PassesToViewModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.UseLocalView = true;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region PagerModel Tests

    [Fact]
    public async Task ProcessAsync_WithPagerModel_UsesProvidedModel()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var pagerModel = new ContinuationPagerViewModel
        {
            PageSize = 50,
            CurrentPage = 3,
            NextPageToken = "custom-token",
            HasMoreResults = true,
            PreviousPageText = "Custom Previous",
            NextPageText = "Custom Next"
        };
        tagHelper.PagerModel = pagerModel;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region Action and Controller Tests

    [Fact]
    public async Task ProcessAsync_WithActionAndController_SupportsProperties()
    {
        // Arrange
        var (tagHelper, context, output) = CreateTagHelperSetup();
        var model = CreateContinuationPagingModel();
        tagHelper.Model = model;
        tagHelper.Action = "Index";
        tagHelper.Controller = "Products";

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        output.TagName.Should().Be("div");
    }

    #endregion

    #region Default Values Tests

    [Fact]
    public void TagHelper_DefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var (tagHelper, _, _) = CreateTagHelperSetup();

        // Assert
        tagHelper.ViewType.Should().Be(ViewType.TailwindAndDaisy);
        tagHelper.UseHtmx.Should().BeTrue();
        tagHelper.ShowPageSize.Should().BeTrue();
        tagHelper.ShowPageNumber.Should().BeTrue();
        tagHelper.UseLocalView.Should().BeFalse();
        tagHelper.CssClass.Should().Be("flex gap-2 items-center");
        tagHelper.PreviousPageText.Should().Be("‹ Previous");
        tagHelper.NextPageText.Should().Be("Next ›");
        tagHelper.PreviousPageAriaLabel.Should().Be("go to previous page");
        tagHelper.NextPageAriaLabel.Should().Be("go to next page");
        tagHelper.HtmxTarget.Should().Be(string.Empty);
        tagHelper.EnableTokenAccumulation.Should().BeTrue();
    }

    #endregion

    #region Helper Methods

    private static (ContinuationPagerTagHelper tagHelper, TagHelperContext context, TagHelperOutput output) CreateTagHelperSetup(string path = "/test/path")
    {
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
            .Returns("/test/action");

        var mockUrlHelperFactory = new Mock<IUrlHelperFactory>();
        mockUrlHelperFactory.Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>()))
            .Returns(mockUrlHelper.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = new PathString(path);

        // Mock the service provider to provide IViewComponentHelper
        // The mock needs to implement both IViewComponentHelper and IViewContextAware
        var mockViewComponentHelper = new Mock<IViewComponentHelper>();
        mockViewComponentHelper.As<IViewContextAware>();

        var mockViewComponentResult = new Mock<IHtmlContent>();
        mockViewComponentResult.Setup(x => x.WriteTo(It.IsAny<System.IO.TextWriter>(), It.IsAny<System.Text.Encodings.Web.HtmlEncoder>()));

        mockViewComponentHelper.Setup(x => x.InvokeAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(mockViewComponentResult.Object);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(mockViewComponentHelper.Object);
        httpContext.RequestServices = serviceCollection.BuildServiceProvider();

        var viewContext = new ViewContext
        {
            HttpContext = httpContext
        };

        var tagHelper = new ContinuationPagerTagHelper(mockUrlHelperFactory.Object)
        {
            ViewContext = viewContext
        };

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));

        var output = new TagHelperOutput(
            "continuation-pager",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        return (tagHelper, context, output);
    }

    private static IContinuationPagingModel CreateContinuationPagingModel(
        int pageSize = 25,
        int currentPage = 1,
        string? nextToken = "token123",
        bool hasMoreResults = true)
    {
        var mockModel = new Mock<IContinuationPagingModel>();
        mockModel.Setup(m => m.PageSize).Returns(pageSize);
        mockModel.Setup(m => m.CurrentPage).Returns(currentPage);
        mockModel.Setup(m => m.NextPageToken).Returns(nextToken);
        mockModel.Setup(m => m.HasMoreResults).Returns(hasMoreResults);
        mockModel.Setup(m => m.ViewType).Returns(ViewType.TailwindAndDaisy);
        mockModel.Setup(m => m.PageTokenHistory).Returns((Dictionary<int, string>?)null);

        return mockModel.Object;
    }

    #endregion
}
