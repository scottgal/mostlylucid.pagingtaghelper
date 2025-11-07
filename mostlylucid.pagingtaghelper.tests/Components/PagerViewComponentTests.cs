using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Moq;
using mostlylucid.pagingtaghelper.Components;
using mostlylucid.pagingtaghelper.Models;
using mostlylucid.pagingtaghelper.Models.TagModels;

namespace mostlylucid.pagingtaghelper.tests.Components;

public class PagerViewComponentTests
{
    [Fact]
    public void Invoke_WithValidModel_ReturnsDefaultView()
    {
        // Arrange
        var viewComponent = CreatePagerViewComponent();
        var model = new PagerViewModel
        {
            Page = 1,
            TotalItems = 100,
            PageSize = 10,
            ViewType = ViewType.TailwindAndDaisy
        };

        // Act
        var result = viewComponent.Invoke(model);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Invoke_ClampsPageToValidRange_WhenPageIsZero()
    {
        // Arrange
        var viewComponent = CreatePagerViewComponent();
        var model = new PagerViewModel
        {
            Page = 0,
            TotalItems = 100,
            PageSize = 10
        };

        // Act
        viewComponent.Invoke(model);

        // Assert
        model.Page.Should().Be(1);
    }

    [Fact]
    public void Invoke_ClampsPageToValidRange_WhenPageExceedsTotalPages()
    {
        // Arrange
        var viewComponent = CreatePagerViewComponent();
        var model = new PagerViewModel
        {
            Page = 20,
            TotalItems = 100,
            PageSize = 10
        };

        // Act
        viewComponent.Invoke(model);

        // Assert
        model.Page.Should().Be(10); // TotalPages = 100 / 10 = 10
    }

    [Fact]
    public void Invoke_WithBootstrapViewType_ReturnsBootstrapView()
    {
        // Arrange
        var viewComponent = CreatePagerViewComponent();
        var model = new PagerViewModel
        {
            Page = 1,
            TotalItems = 100,
            PageSize = 10,
            ViewType = ViewType.Bootstrap
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
        var viewComponent = CreatePagerViewComponent();
        var model = new PagerViewModel
        {
            Page = 1,
            TotalItems = 100,
            PageSize = 10,
            ViewType = ViewType.Plain
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
        var viewComponent = CreatePagerViewComponent();
        var model = new PagerViewModel
        {
            Page = 1,
            TotalItems = 100,
            PageSize = 10,
            ViewType = ViewType.TailwindAndDaisy
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
        var viewComponent = CreatePagerViewComponent();
        var mockPagingModel = new Mock<IPagingSearchModel>();
        mockPagingModel.Setup(m => m.SearchTerm).Returns("test");
        mockPagingModel.Setup(m => m.LinkUrl).Returns("/test");
        mockPagingModel.Setup(m => m.Page).Returns(1);
        mockPagingModel.Setup(m => m.TotalItems).Returns(100);
        mockPagingModel.Setup(m => m.PageSize).Returns(10);

        var model = new PagerViewModel
        {
            Page = 1,
            TotalItems = 100,
            PageSize = 10,
            Model = mockPagingModel.Object
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
        var viewComponent = CreatePagerViewComponent();
        var mockPagingModel = new Mock<IPagingModel>();
        mockPagingModel.Setup(m => m.LinkUrl).Returns("/products");
        mockPagingModel.Setup(m => m.Page).Returns(1);
        mockPagingModel.Setup(m => m.TotalItems).Returns(100);
        mockPagingModel.Setup(m => m.PageSize).Returns(10);

        var model = new PagerViewModel
        {
            Page = 1,
            TotalItems = 100,
            PageSize = 10,
            Model = mockPagingModel.Object
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
        var viewComponent = CreatePagerViewComponent();
        var mockPagingModel = new Mock<IPagingModel>();
        mockPagingModel.Setup(m => m.LinkUrl).Returns("/products");
        mockPagingModel.Setup(m => m.Page).Returns(1);
        mockPagingModel.Setup(m => m.TotalItems).Returns(100);
        mockPagingModel.Setup(m => m.PageSize).Returns(10);

        var model = new PagerViewModel
        {
            Page = 1,
            TotalItems = 100,
            PageSize = 10,
            LinkUrl = "/custom",
            Model = mockPagingModel.Object
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
        var viewComponent = CreatePagerViewComponent();
        var mockPagingModel = new Mock<IPagingSearchModel>();
        mockPagingModel.Setup(m => m.SearchTerm).Returns("test");
        mockPagingModel.Setup(m => m.LinkUrl).Returns("/test");
        mockPagingModel.Setup(m => m.Page).Returns(1);
        mockPagingModel.Setup(m => m.TotalItems).Returns(100);
        mockPagingModel.Setup(m => m.PageSize).Returns(10);

        var model = new PagerViewModel
        {
            Page = 1,
            TotalItems = 100,
            PageSize = 10,
            SearchTerm = "custom",
            Model = mockPagingModel.Object
        };

        // Act
        viewComponent.Invoke(model);

        // Assert
        model.SearchTerm.Should().Be("custom");
    }

    private static PagerViewComponent CreatePagerViewComponent()
    {
        var component = new PagerViewComponent();
        return component;
    }
}
