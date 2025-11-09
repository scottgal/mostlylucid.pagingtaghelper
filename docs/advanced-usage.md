# Advanced Usage

This guide covers advanced scenarios and power features of the Paging TagHelper library.

## Table of Contents

- [Multiple Pagers on One Page](#multiple-pagers-on-one-page)
- [Pagination with Search and Filtering](#pagination-with-search-and-filtering)
- [Pagination with Sorting](#pagination-with-sorting)
- [HTMX Advanced Patterns](#htmx-advanced-patterns)
- [Dynamic Page Size Selection](#dynamic-page-size-selection)
- [URL Configuration](#url-configuration)
- [Custom Page Size Steps](#custom-page-size-steps)
- [Combining Multiple Features](#combining-multiple-features)
- [Performance Optimization](#performance-optimization)
- [Error Handling](#error-handling)
- [Security Considerations](#security-considerations)

## Multiple Pagers on One Page

You can have multiple independent pagers on the same page with different datasets.

### Setup

Give each pager a unique ID and different models:

```csharp
public class DashboardViewModel
{
    public ProductListModel Products { get; set; }
    public OrderListModel Orders { get; set; }
}

public class ProductListModel : IPagingModel
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public List<Product> Items { get; set; }
}

public class OrderListModel : IPagingModel
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public List<Order> Items { get; set; }
}
```

### Controller

```csharp
public async Task<IActionResult> Dashboard(
    int productsPage = 1,
    int productsPageSize = 10,
    int ordersPage = 1,
    int ordersPageSize = 5)
{
    var model = new DashboardViewModel
    {
        Products = new ProductListModel
        {
            Page = productsPage,
            PageSize = productsPageSize,
            TotalItems = await _db.Products.CountAsync(),
            Items = await _db.Products
                .Skip((productsPage - 1) * productsPageSize)
                .Take(productsPageSize)
                .ToListAsync()
        },
        Orders = new OrderListModel
        {
            Page = ordersPage,
            PageSize = ordersPageSize,
            TotalItems = await _db.Orders.CountAsync(),
            Items = await _db.Orders
                .Skip((ordersPage - 1) * ordersPageSize)
                .Take(ordersPageSize)
                .ToListAsync()
        }
    };

    return View(model);
}
```

### View

```html
@model DashboardViewModel

<section class="products-section">
    <h2>Products</h2>
    <div id="products-list">
        @foreach (var product in Model.Products.Items)
        {
            <div>@product.Name</div>
        }
    </div>

    <paging model="Model.Products"
            id="products-pager"
            link-url="/Dashboard"
            htmx-target="#products-list" />
</section>

<section class="orders-section">
    <h2>Orders</h2>
    <div id="orders-list">
        @foreach (var order in Model.Orders.Items)
        {
            <div>Order #@order.Id</div>
        }
    </div>

    <paging model="Model.Orders"
            id="orders-pager"
            link-url="/Dashboard"
            htmx-target="#orders-list" />
</section>
```

**Note:** Query parameters are automatically namespaced by the pager to prevent conflicts.

## Pagination with Search and Filtering

Preserve search terms and filters across pagination.

### ViewModel

```csharp
public class ProductSearchViewModel : IPagingModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalItems { get; set; }

    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public List<Product> Products { get; set; } = new();
}
```

### Controller

```csharp
public async Task<IActionResult> Search(
    string? searchTerm,
    string? category,
    decimal? minPrice,
    decimal? maxPrice,
    int page = 1,
    int pageSize = 20)
{
    var query = _db.Products.AsQueryable();

    // Apply filters
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        query = query.Where(p => p.Name.Contains(searchTerm) ||
                                p.Description.Contains(searchTerm));
    }

    if (!string.IsNullOrWhiteSpace(category))
    {
        query = query.Where(p => p.Category == category);
    }

    if (minPrice.HasValue)
    {
        query = query.Where(p => p.Price >= minPrice.Value);
    }

    if (maxPrice.HasValue)
    {
        query = query.Where(p => p.Price <= maxPrice.Value);
    }

    var totalItems = await query.CountAsync();
    var products = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var model = new ProductSearchViewModel
    {
        Page = page,
        PageSize = pageSize,
        TotalItems = totalItems,
        SearchTerm = searchTerm,
        Category = category,
        MinPrice = minPrice,
        MaxPrice = maxPrice,
        Products = products
    };

    if (Request.Headers["HX-Request"] == "true")
    {
        return PartialView("_ProductList", model);
    }

    return View(model);
}
```

### View

```html
@model ProductSearchViewModel

<form method="get" hx-get="/Products/Search" hx-target="#results">
    <input type="text" name="searchTerm" value="@Model.SearchTerm" placeholder="Search..." />

    <select name="category">
        <option value="">All Categories</option>
        <option value="Electronics" selected="@(Model.Category == "Electronics")">Electronics</option>
        <option value="Clothing" selected="@(Model.Category == "Clothing")">Clothing</option>
    </select>

    <input type="number" name="minPrice" value="@Model.MinPrice" placeholder="Min Price" />
    <input type="number" name="maxPrice" value="@Model.MaxPrice" placeholder="Max Price" />

    <button type="submit">Search</button>
</form>

<div id="results">
    <partial name="_ProductList" model="Model" />
</div>
```

**_ProductList.cshtml:**
```html
@model ProductSearchViewModel

@foreach (var product in Model.Products)
{
    <div class="product-card">
        <h3>@product.Name</h3>
        <p>@product.Price.ToString("C")</p>
    </div>
}

<paging model="Model"
        search-term="@Model.SearchTerm"
        htmx-target="#results" />
```

**Note:** The pager automatically preserves ALL query parameters, including your custom filters. You can explicitly pass `search-term` for backward compatibility, but it's not required.

## Pagination with Sorting

Combine pagination with sortable column headers.

### ViewModel

```csharp
public class SortableProductListViewModel : IPagingModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public int TotalItems { get; set; }

    public string? OrderBy { get; set; }
    public bool Descending { get; set; }

    public List<Product> Products { get; set; } = new();
}
```

### Controller

```csharp
public async Task<IActionResult> Index(
    string? orderBy,
    bool descending = false,
    int page = 1,
    int pageSize = 25)
{
    var query = _db.Products.AsQueryable();

    // Apply sorting
    query = orderBy switch
    {
        "Name" => descending ? query.OrderByDescending(p => p.Name)
                             : query.OrderBy(p => p.Name),
        "Price" => descending ? query.OrderByDescending(p => p.Price)
                              : query.OrderBy(p => p.Price),
        "Category" => descending ? query.OrderByDescending(p => p.Category)
                                 : query.OrderBy(p => p.Category),
        _ => query.OrderBy(p => p.Id)
    };

    var model = new SortableProductListViewModel
    {
        Page = page,
        PageSize = pageSize,
        TotalItems = await query.CountAsync(),
        OrderBy = orderBy,
        Descending = descending,
        Products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync()
    };

    return View(model);
}
```

### View

```html
@model SortableProductListViewModel

<div id="product-table">
    <table>
        <thead>
            <tr>
                <sortable-header column="Name"
                               current-order-by="@Model.OrderBy"
                               descending="@Model.Descending"
                               hx-target="#product-table">
                    Product Name
                </sortable-header>
                <sortable-header column="Price"
                               current-order-by="@Model.OrderBy"
                               descending="@Model.Descending"
                               hx-target="#product-table">
                    Price
                </sortable-header>
                <sortable-header column="Category"
                               current-order-by="@Model.OrderBy"
                               descending="@Model.Descending"
                               hx-target="#product-table">
                    Category
                </sortable-header>
            </tr>
        </thead>
        <tbody>
            @foreach (var product in Model.Products)
            {
                <tr>
                    <td>@product.Name</td>
                    <td>@product.Price.ToString("C")</td>
                    <td>@product.Category</td>
                </tr>
            }
        </tbody>
    </table>

    <paging model="Model"
            order-by="@Model.OrderBy"
            descending="@Model.Descending"
            htmx-target="#product-table" />
</div>
```

## HTMX Advanced Patterns

### Partial Page Updates

Update only specific sections:

```html
<div id="content-wrapper">
    <div id="sidebar">
        <!-- Sidebar content -->
    </div>

    <div id="main-content">
        <partial name="_ProductList" model="Model" />
    </div>
</div>

<paging model="Model"
        htmx-target="#main-content"
        hx-swap="innerHTML" />
```

### Loading Indicators

```html
<div id="results" hx-indicator="#spinner">
    <partial name="_ProductList" model="Model" />
</div>

<div id="spinner" class="htmx-indicator">
    <div class="spinner-border"></div>
    Loading...
</div>

<paging model="Model" htmx-target="#results" />
```

### Scroll to Top After Navigation

```html
<div id="results" hx-target="this" hx-swap="outerHTML show:top">
    <partial name="_ProductList" model="Model" />
</div>

<paging model="Model" htmx-target="#results" />
```

### History Support

```html
<paging model="Model"
        htmx-target="#results"
        hx-push-url="true" />
```

### Request Indicators

```html
<paging model="Model"
        htmx-target="#results"
        hx-indicator=".progress-bar" />

<div class="progress-bar htmx-indicator">
    <div class="progress-bar-fill"></div>
</div>
```

## Dynamic Page Size Selection

Allow users to change items per page dynamically.

### Using Standalone PageSize Component

```html
<div class="controls">
    <page-size model="Model"
               hx-target="#results"
               page-size-steps="10,25,50,100" />
</div>

<div id="results">
    <partial name="_ProductList" model="Model" />
</div>

<paging model="Model"
        show-pagesize="false"
        htmx-target="#results" />
```

### Custom Page Size Options

```csharp
public class CustomPagingViewModel : IPagingModel
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }

    // Custom page sizes
    public List<int> PageSizes { get; set; } = new() { 5, 10, 25, 50, 100, 250, 500 };
}
```

## URL Configuration

### Explicit Link URL

```html
<paging model="Model" link-url="/Products/List" />
```

### MVC Action/Controller

```html
<paging model="Model"
        controller="Products"
        action="Search" />
```

### With Area

```csharp
// Controller
[Area("Admin")]
public class ProductsController : Controller
{
    // ...
}
```

```html
<paging model="Model"
        area="Admin"
        controller="Products"
        action="Index" />
```

### Absolute URLs

```html
<paging model="Model"
        link-url="https://example.com/api/products" />
```

## Custom Page Size Steps

Control which page size options appear:

### In Razor

```html
<paging model="Model"
        page-size-steps="5,10,20,50,100,500" />
```

### In ViewModel

```csharp
public class ProductListViewModel : IPagingModel
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public List<int> PageSizes { get; set; } = new() { 15, 30, 60, 120 };
}
```

### With Maximum Limit

```html
<paging model="Model"
        page-size-steps="10,25,50,100,250"
        max-page-size="250" />
```

## Combining Multiple Features

### Kitchen Sink Example

```csharp
public class AdvancedProductViewModel : IPagingModel
{
    // Paging
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public int TotalItems { get; set; }

    // Sorting
    public string? OrderBy { get; set; }
    public bool Descending { get; set; }

    // Filtering
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public bool InStock { get; set; }

    // Data
    public List<Product> Products { get; set; } = new();

    // Custom page sizes
    public List<int> PageSizes { get; set; } = new() { 10, 25, 50, 100 };
}
```

**Controller:**
```csharp
public async Task<IActionResult> AdvancedSearch(
    string? searchTerm,
    string? category,
    bool inStock = false,
    string? orderBy = "Name",
    bool descending = false,
    int page = 1,
    int pageSize = 25)
{
    var query = _db.Products.AsQueryable();

    // Filters
    if (!string.IsNullOrWhiteSpace(searchTerm))
        query = query.Where(p => p.Name.Contains(searchTerm));

    if (!string.IsNullOrWhiteSpace(category))
        query = query.Where(p => p.Category == category);

    if (inStock)
        query = query.Where(p => p.Stock > 0);

    // Sorting
    query = ApplySorting(query, orderBy, descending);

    // Pagination
    var totalItems = await query.CountAsync();
    var products = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var model = new AdvancedProductViewModel
    {
        Page = page,
        PageSize = pageSize,
        TotalItems = totalItems,
        OrderBy = orderBy,
        Descending = descending,
        SearchTerm = searchTerm,
        Category = category,
        InStock = inStock,
        Products = products
    };

    return Request.Headers["HX-Request"] == "true"
        ? PartialView("_ProductTable", model)
        : View(model);
}
```

**View:**
```html
@model AdvancedProductViewModel

<div id="product-table">
    <form method="get" hx-get="/Products/AdvancedSearch" hx-target="#product-table">
        <input type="text" name="searchTerm" value="@Model.SearchTerm" />
        <select name="category">
            <option value="">All</option>
            <option value="Electronics">Electronics</option>
        </select>
        <label>
            <input type="checkbox" name="inStock" checked="@Model.InStock" />
            In Stock Only
        </label>
        <button type="submit">Search</button>
    </form>

    <table>
        <thead>
            <tr>
                <sortable-header column="Name"
                               current-order-by="@Model.OrderBy"
                               descending="@Model.Descending"
                               hx-target="#product-table">
                    Name
                </sortable-header>
                <sortable-header column="Price"
                               current-order-by="@Model.OrderBy"
                               descending="@Model.Descending"
                               hx-target="#product-table">
                    Price
                </sortable-header>
                <th>Stock</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var product in Model.Products)
            {
                <tr>
                    <td>@product.Name</td>
                    <td>@product.Price.ToString("C")</td>
                    <td>@product.Stock</td>
                </tr>
            }
        </tbody>
    </table>

    <paging model="Model"
            order-by="@Model.OrderBy"
            descending="@Model.Descending"
            search-term="@Model.SearchTerm"
            htmx-target="#product-table"
            language="en"
            summary-template="Showing {startItem}-{endItem} of {totalItems} products" />
</div>
```

## Performance Optimization

### Use Queryable Extension Methods

The library includes helper extensions for `IQueryable`:

```csharp
using mostlylucid.pagingtaghelper.Extensions;

public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
{
    var query = _db.Products
        .Where(p => p.IsActive)
        .OrderBy(p => p.Name);

    // Extension method handles Skip/Take automatically
    var products = await query.ToPagedListAsync(page, pageSize);

    var model = new ProductListViewModel
    {
        Page = page,
        PageSize = pageSize,
        TotalItems = products.TotalItems,
        Products = products.Items
    };

    return View(model);
}
```

### Caching Total Count

For large datasets, cache the total count:

```csharp
private async Task<int> GetTotalProductCountAsync()
{
    return await _cache.GetOrCreateAsync("ProductCount", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        return await _db.Products.CountAsync();
    });
}
```

### Database Indexing

Ensure proper indexes on sorted/filtered columns:

```csharp
// Entity Framework Core
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>()
        .HasIndex(p => p.Name);

    modelBuilder.Entity<Product>()
        .HasIndex(p => p.Price);

    modelBuilder.Entity<Product>()
        .HasIndex(p => new { p.Category, p.Price });
}
```

## Error Handling

### Invalid Page Numbers

```csharp
public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
{
    var totalItems = await _db.Products.CountAsync();
    var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

    // Redirect to last valid page
    if (page > totalPages && totalPages > 0)
    {
        return RedirectToAction(nameof(Index), new { page = totalPages, pageSize });
    }

    // Redirect to first page if negative
    if (page < 1)
    {
        return RedirectToAction(nameof(Index), new { page = 1, pageSize });
    }

    // Continue with normal logic...
}
```

### Invalid Page Sizes

```csharp
public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
{
    // Enforce limits
    pageSize = Math.Max(5, Math.Min(pageSize, 500));

    // Continue with normal logic...
}
```

## Security Considerations

### SQL Injection Protection

Always use parameterized queries. With Entity Framework Core, this is automatic:

```csharp
// ✅ Safe - parameterized
query = query.Where(p => p.Name.Contains(searchTerm));

// ❌ Dangerous - SQL injection risk
query = query.FromSqlRaw($"SELECT * FROM Products WHERE Name LIKE '%{searchTerm}%'");
```

### XSS Protection

Always encode user input in views. Razor does this automatically:

```html
<!-- ✅ Safe - Razor encodes automatically -->
<h3>@product.Name</h3>

<!-- ❌ Dangerous - bypasses encoding -->
<h3>@Html.Raw(product.Name)</h3>
```

### Parameter Validation

```csharp
public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
{
    // Validate inputs
    if (page < 1) page = 1;
    if (pageSize < 1) pageSize = 25;
    if (pageSize > 500) pageSize = 500; // Max limit

    // Continue...
}
```

### Rate Limiting

For public APIs, implement rate limiting:

```csharp
[EnableRateLimiting("fixed")]
public async Task<IActionResult> PublicApi(int page = 1)
{
    // Your logic here
}
```

## See Also

- [Getting Started](getting-started.md)
- [Pager TagHelper](pager-taghelper.md)
- [PageSize TagHelper](pagesize-taghelper.md)
- [SortableHeader TagHelper](sortable-header-taghelper.md)
- [JavaScript Modes](javascript-modes.md)
- [Custom Views](custom-views.md)
- [Localization](localization.md)
