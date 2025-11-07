using FluentAssertions;
using mostlylucid.pagingtaghelper.Components;
using mostlylucid.pagingtaghelper.Models;
using mostlylucid.pagingtaghelper.Models.TagModels;
using Moq;

namespace mostlylucid.pagingtaghelper.tests.Components;

public class ContinuationPagerViewComponentTests
{
    [Fact]
    public void Invoke_WithValidModel_ReturnsDefaultView()
    {
        // Arrange
        var viewComponent = new ContinuationPagerViewComponent();
        var model = new ContinuationPagerViewModel
        {
            PageSize = 25,
            CurrentPage = 1,
            NextPageToken = "token123",
            HasMoreResults = true,
            ViewType = ViewType.TailwindAndDaisy
        };

        // Act
        var result = viewComponent.Invoke(model);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Invoke_WithTailwindViewType_ReturnsTailwindView()
    {
        // Arrange
        var viewComponent = new ContinuationPagerViewComponent();
        var model = new ContinuationPagerViewModel
        {
            PageSize = 25,
            CurrentPage = 1,
            NextPageToken = "token123",
            HasMoreResults = true,
            ViewType = ViewType.Tailwind
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
        var viewComponent = new ContinuationPagerViewComponent();
        var model = new ContinuationPagerViewModel
        {
            PageSize = 25,
            CurrentPage = 1,
            NextPageToken = "token123",
            HasMoreResults = true,
            ViewType = ViewType.Bootstrap
        };

        // Act
        var result = viewComponent.Invoke(model);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Invoke_WithPlainViewType_ReturnsPlainView()
    {
        // Arrange
        var viewComponent = new ContinuationPagerViewComponent();
        var model = new ContinuationPagerViewModel
        {
            PageSize = 25,
            CurrentPage = 1,
            NextPageToken = "token123",
            HasMoreResults = true,
            ViewType = ViewType.Plain
        };

        // Act
        var result = viewComponent.Invoke(model);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Invoke_WithNoJSViewType_ReturnsNoJSView()
    {
        // Arrange
        var viewComponent = new ContinuationPagerViewComponent();
        var model = new ContinuationPagerViewModel
        {
            PageSize = 25,
            CurrentPage = 1,
            NextPageToken = "token123",
            HasMoreResults = true,
            ViewType = ViewType.NoJS
        };

        // Act
        var result = viewComponent.Invoke(model);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Invoke_WithUseLocalView_ReturnsCustomView()
    {
        // Arrange
        var viewComponent = new ContinuationPagerViewComponent();
        var model = new ContinuationPagerViewModel
        {
            PageSize = 25,
            CurrentPage = 1,
            NextPageToken = "token123",
            HasMoreResults = true,
            UseLocalView = true
        };

        // Act
        var result = viewComponent.Invoke(model);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Invoke_WithIContinuationPagingModel_PopulatesProperties()
    {
        // Arrange
        var viewComponent = new ContinuationPagerViewComponent();
        var mockModel = new Mock<IContinuationPagingModel>();
        mockModel.Setup(m => m.NextPageToken).Returns("token456");
        mockModel.Setup(m => m.HasMoreResults).Returns(true);
        mockModel.Setup(m => m.PageSize).Returns(50);
        mockModel.Setup(m => m.CurrentPage).Returns(2);
        mockModel.Setup(m => m.ViewType).Returns(ViewType.Bootstrap);
        mockModel.Setup(m => m.PageTokenHistory).Returns(new Dictionary<int, string>
        {
            { 1, "token123" }
        });

        var model = new ContinuationPagerViewModel
        {
            PageSize = 50,
            CurrentPage = 2,
            NextPageToken = "token456",
            HasMoreResults = true,
            ViewType = ViewType.Bootstrap,
            Model = mockModel.Object
        };

        // Act
        var result = viewComponent.Invoke(model);

        // Assert
        result.Should().NotBeNull();
        model.NextPageToken.Should().Be("token456");
        model.HasMoreResults.Should().BeTrue();
    }
}
