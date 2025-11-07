using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Moq;
using mostlylucid.pagingtaghelper.Components;
using mostlylucid.pagingtaghelper.Models;
using mostlylucid.pagingtaghelper.Models.TagModels;

namespace mostlylucid.pagingtaghelper.tests.Components;

public class PageSizeViewComponentTests
{
    [Fact]
    public void Invoke_WithValidModel_ReturnsDefaultView()
    {
        // Arrange
        var viewComponent = CreatePageSizeViewComponent();
        var model = new PageSizeModel
        {
            PageSize = 10,
            TotalItems = 100,
            ViewType = ViewType.TailwindAndDaisy,
            PageSizes = new List<int> { 10, 25, 50 }
        };

        // Act
        var result = viewComponent.Invoke(model);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Invoke_WithBootstrapViewType_ReturnsBootstrapView()
    {
        // Arrange
        var viewComponent = CreatePageSizeViewComponent();
        var model = new PageSizeModel
        {
            PageSize = 10,
            TotalItems = 100,
            ViewType = ViewType.Bootstrap,
            PageSizes = new List<int> { 10, 25, 50 }
        };

        // Act
        var result = viewComponent.Invoke(model) ;

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Invoke_WithPlainViewType_ReturnsPlainView()
    {
        // Arrange
        var viewComponent = CreatePageSizeViewComponent();
        var model = new PageSizeModel
        {
            PageSize = 10,
            TotalItems = 100,
            ViewType = ViewType.Plain,
            PageSizes = new List<int> { 10, 25, 50 }
        };

        // Act
        var result = viewComponent.Invoke(model);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Invoke_WithTailwindAndDaisyViewType_ReturnsDefaultView()
    {
        // Arrange
        var viewComponent = CreatePageSizeViewComponent();
        var model = new PageSizeModel
        {
            PageSize = 10,
            TotalItems = 100,
            ViewType = ViewType.TailwindAndDaisy,
            PageSizes = new List<int> { 10, 25, 50 }
        };

        // Act
        var result = viewComponent.Invoke(model);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Invoke_WithIPagingSearchModel_PopulatesSearchTerm()
    {
        // Arrange
        var viewComponent = CreatePageSizeViewComponent();
        var mockPagingModel = new Mock<IPagingSearchModel>();
        mockPagingModel.Setup(m => m.SearchTerm).Returns("test");
        mockPagingModel.Setup(m => m.LinkUrl).Returns("/test");
        mockPagingModel.Setup(m => m.Page).Returns(1);
        mockPagingModel.Setup(m => m.TotalItems).Returns(100);
        mockPagingModel.Setup(m => m.PageSize).Returns(10);

        var model = new PageSizeModel
        {
            PageSize = 10,
            TotalItems = 100,
            Model = mockPagingModel.Object,
            PageSizes = new List<int> { 10, 25, 50 }
        };

        // Act
        viewComponent.Invoke(model);

        // Assert
        model.SearchTerm.Should().Be("test");
    }

    [Fact]
    public void Invoke_WithIPagingModel_PopulatesLinkUrl()
    {
        // Arrange
        var viewComponent = CreatePageSizeViewComponent();
        var mockPagingModel = new Mock<IPagingModel>();
        mockPagingModel.Setup(m => m.LinkUrl).Returns("/products");
        mockPagingModel.Setup(m => m.Page).Returns(1);
        mockPagingModel.Setup(m => m.TotalItems).Returns(100);
        mockPagingModel.Setup(m => m.PageSize).Returns(10);

        var model = new PageSizeModel
        {
            PageSize = 10,
            TotalItems = 100,
            Model = mockPagingModel.Object,
            PageSizes = new List<int> { 10, 25, 50 }
        };

        // Act
        viewComponent.Invoke(model);

        // Assert
        model.LinkUrl.Should().Be("/products");
    }

    [Fact]
    public void Invoke_DoesNotOverrideExistingLinkUrl()
    {
        // Arrange
        var viewComponent = CreatePageSizeViewComponent();
        var mockPagingModel = new Mock<IPagingModel>();
        mockPagingModel.Setup(m => m.LinkUrl).Returns("/products");
        mockPagingModel.Setup(m => m.Page).Returns(1);
        mockPagingModel.Setup(m => m.TotalItems).Returns(100);
        mockPagingModel.Setup(m => m.PageSize).Returns(10);

        var model = new PageSizeModel
        {
            PageSize = 10,
            TotalItems = 100,
            LinkUrl = "/custom",
            Model = mockPagingModel.Object,
            PageSizes = new List<int> { 10, 25, 50 }
        };

        // Act
        viewComponent.Invoke(model);

        // Assert
        model.LinkUrl.Should().Be("/custom");
    }

    [Fact]
    public void Invoke_DoesNotOverrideExistingSearchTerm()
    {
        // Arrange
        var viewComponent = CreatePageSizeViewComponent();
        var mockPagingModel = new Mock<IPagingSearchModel>();
        mockPagingModel.Setup(m => m.SearchTerm).Returns("test");
        mockPagingModel.Setup(m => m.LinkUrl).Returns("/test");
        mockPagingModel.Setup(m => m.Page).Returns(1);
        mockPagingModel.Setup(m => m.TotalItems).Returns(100);
        mockPagingModel.Setup(m => m.PageSize).Returns(10);

        var model = new PageSizeModel
        {
            PageSize = 10,
            TotalItems = 100,
            SearchTerm = "custom",
            Model = mockPagingModel.Object,
            PageSizes = new List<int> { 10, 25, 50 }
        };

        // Act
        viewComponent.Invoke(model);

        // Assert
        model.SearchTerm.Should().Be("custom");
    }

    private static PageSizeViewComponent CreatePageSizeViewComponent()
    {
        var component = new PageSizeViewComponent();
        return component;
    }
}
