using FluentAssertions;
using mostlylucid.pagingtaghelper.Extensions;

namespace mostlylucid.pagingtaghelper.tests.Extensions;

public class QueryableExtensionsTests
{
    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    [Fact]
    public void OrderByField_WithExpression_OrdersAscending()
    {
        // Arrange
        var items = new[]
        {
            new TestEntity { Id = 3, Name = "C" },
            new TestEntity { Id = 1, Name = "A" },
            new TestEntity { Id = 2, Name = "B" }
        }.AsQueryable();

        // Act
        var result = items.OrderByField(x => x.Id, descending: false).ToList();

        // Assert
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);
        result[2].Id.Should().Be(3);
    }

    [Fact]
    public void OrderByField_WithExpression_OrdersDescending()
    {
        // Arrange
        var items = new[]
        {
            new TestEntity { Id = 1, Name = "A" },
            new TestEntity { Id = 3, Name = "C" },
            new TestEntity { Id = 2, Name = "B" }
        }.AsQueryable();

        // Act
        var result = items.OrderByField(x => x.Id, descending: true).ToList();

        // Assert
        result[0].Id.Should().Be(3);
        result[1].Id.Should().Be(2);
        result[2].Id.Should().Be(1);
    }

    [Fact]
    public void OrderByField_WithStringPropertyName_OrdersAscending()
    {
        // Arrange
        var items = new[]
        {
            new TestEntity { Id = 1, Name = "Charlie" },
            new TestEntity { Id = 2, Name = "Alice" },
            new TestEntity { Id = 3, Name = "Bob" }
        }.AsQueryable();

        // Act
        var result = items.OrderByField("Name", descending: false).ToList();

        // Assert
        result[0].Name.Should().Be("Alice");
        result[1].Name.Should().Be("Bob");
        result[2].Name.Should().Be("Charlie");
    }

    [Fact]
    public void OrderByField_WithStringPropertyName_OrdersDescending()
    {
        // Arrange
        var items = new[]
        {
            new TestEntity { Id = 1, Name = "Alice" },
            new TestEntity { Id = 2, Name = "Charlie" },
            new TestEntity { Id = 3, Name = "Bob" }
        }.AsQueryable();

        // Act
        var result = items.OrderByField("Name", descending: true).ToList();

        // Assert
        result[0].Name.Should().Be("Charlie");
        result[1].Name.Should().Be("Bob");
        result[2].Name.Should().Be("Alice");
    }

    [Fact]
    public void OrderByField_WithCaseInsensitivePropertyName_Works()
    {
        // Arrange
        var items = new[]
        {
            new TestEntity { Id = 1, Name = "Charlie" },
            new TestEntity { Id = 2, Name = "Alice" },
            new TestEntity { Id = 3, Name = "Bob" }
        }.AsQueryable();

        // Act
        var result = items.OrderByField("name", descending: false).ToList();

        // Assert
        result[0].Name.Should().Be("Alice");
        result[1].Name.Should().Be("Bob");
        result[2].Name.Should().Be("Charlie");
    }

    [Fact]
    public void OrderByField_WithInvalidPropertyName_ThrowsArgumentException()
    {
        // Arrange
        var items = new[]
        {
            new TestEntity { Id = 1, Name = "Alice" }
        }.AsQueryable();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => items.OrderByField("InvalidProperty"));
    }

    [Fact]
    public void OrderByField_WithEmptyPropertyName_ReturnsUnchangedQueryable()
    {
        // Arrange
        var items = new[]
        {
            new TestEntity { Id = 3, Name = "C" },
            new TestEntity { Id = 1, Name = "A" },
            new TestEntity { Id = 2, Name = "B" }
        }.AsQueryable();

        // Act
        var result = items.OrderByField("", descending: false).ToList();

        // Assert
        result[0].Id.Should().Be(3);
        result[1].Id.Should().Be(1);
        result[2].Id.Should().Be(2);
    }

    [Fact]
    public void OrderByField_WithWhitespacePropertyName_ReturnsUnchangedQueryable()
    {
        // Arrange
        var items = new[]
        {
            new TestEntity { Id = 3, Name = "C" },
            new TestEntity { Id = 1, Name = "A" },
            new TestEntity { Id = 2, Name = "B" }
        }.AsQueryable();

        // Act
        var result = items.OrderByField("   ", descending: false).ToList();

        // Assert
        result[0].Id.Should().Be(3);
        result[1].Id.Should().Be(1);
        result[2].Id.Should().Be(2);
    }

    [Fact]
    public void OrderByField_IEnumerable_WithStringPropertyName_OrdersAscending()
    {
        // Arrange
        var items = new[]
        {
            new TestEntity { Id = 1, Name = "Charlie", Price = 30m },
            new TestEntity { Id = 2, Name = "Alice", Price = 10m },
            new TestEntity { Id = 3, Name = "Bob", Price = 20m }
        };

        // Act
        var result = items.OrderByField("Price", descending: false).ToList();

        // Assert
        result[0].Price.Should().Be(10m);
        result[1].Price.Should().Be(20m);
        result[2].Price.Should().Be(30m);
    }

    [Fact]
    public void OrderByField_IEnumerable_WithStringPropertyName_OrdersDescending()
    {
        // Arrange
        var items = new[]
        {
            new TestEntity { Id = 1, Name = "Alice", Price = 10m },
            new TestEntity { Id = 2, Name = "Charlie", Price = 30m },
            new TestEntity { Id = 3, Name = "Bob", Price = 20m }
        };

        // Act
        var result = items.OrderByField("Price", descending: true).ToList();

        // Assert
        result[0].Price.Should().Be(30m);
        result[1].Price.Should().Be(20m);
        result[2].Price.Should().Be(10m);
    }

    [Fact]
    public void OrderByField_IEnumerable_WithInvalidPropertyName_ThrowsArgumentException()
    {
        // Arrange
        var items = new[]
        {
            new TestEntity { Id = 1, Name = "Alice" }
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => items.OrderByField("InvalidProperty").ToList());
    }

    [Fact]
    public void OrderByField_WithDateTimeProperty_OrdersCorrectly()
    {
        // Arrange
        var items = new[]
        {
            new TestEntity { Id = 1, CreatedDate = new DateTime(2023, 3, 1) },
            new TestEntity { Id = 2, CreatedDate = new DateTime(2023, 1, 1) },
            new TestEntity { Id = 3, CreatedDate = new DateTime(2023, 2, 1) }
        }.AsQueryable();

        // Act
        var result = items.OrderByField("CreatedDate", descending: false).ToList();

        // Assert
        result[0].CreatedDate.Should().Be(new DateTime(2023, 1, 1));
        result[1].CreatedDate.Should().Be(new DateTime(2023, 2, 1));
        result[2].CreatedDate.Should().Be(new DateTime(2023, 3, 1));
    }
}
