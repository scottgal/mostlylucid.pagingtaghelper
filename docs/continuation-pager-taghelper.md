# Continuation Pager TagHelper

The `<continuation-pager>` TagHelper provides navigation controls for continuation token-based pagination systems like Cosmos DB, Azure Table Storage, DynamoDB, and other NoSQL databases that don't use traditional page numbers.

## Table of Contents

- [When to Use Continuation Pager](#when-to-use-continuation-pager)
- [Basic Usage](#basic-usage)
- [Attributes Reference](#attributes-reference)
- [Token Management](#token-management)
- [Database-Specific Examples](#database-specific-examples)
- [JavaScript Modes](#javascript-modes)
- [Styling Options](#styling-options)
- [Advanced Features](#advanced-features)

## When to Use Continuation Pager

Use the Continuation Pager when:

1. **Cosmos DB**: Using continuation tokens for efficient querying
2. **Azure Table Storage**: Using `TableContinuationToken`
3. **AWS DynamoDB**: Using `LastEvaluatedKey` for pagination
4. **MongoDB**: Using cursor-based pagination
5. **Cassandra**: Using paging state tokens
6. **Any NoSQL database**: That uses continuation tokens instead of offset/limit

## Basic Usage

### Simple Previous/Next Navigation

```html
<continuation-pager
    next-page-token="@Model.NextPageToken"
    has-more-results="@Model.HasMoreResults"
    current-page="@Model.CurrentPage"
    page-size="@Model.PageSize" />
```

### With IContinuationPagingModel

```html
<continuation-pager model="Model" />
```

## Attributes Reference

### Core Attributes

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `model` | `IContinuationPagingModel?` | null | The continuation paging model |
| `next-page-token` | `string?` | null | Token for the next page |
| `has-more-results` | `bool?` | false | Whether more results are available |
| `current-page` | `int?` | 1 | Current page number (for display only) |
| `page-size` | `int?` | 10 | Number of items per page |
| `page-token-history` | `string?` | null | JSON-serialized token history for backward navigation |

### Display Control

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `show-page-number` | `bool` | true | Whether to show the current page number badge |
| `show-pagesize` | `bool` | true | Whether to show the page size selector |
| `previous-page-text` | `string` | "‹ Previous" | Text for previous button |
| `next-page-text` | `string` | "Next ›" | Text for next button |
| `previous-page-aria-label` | `string` | "go to previous page" | ARIA label for previous button |
| `next-page-aria-label` | `string` | "go to next page" | ARIA label for next button |

### Styling and Behavior

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `view-type` | `ViewType` | TailwindAndDaisy | UI framework |
| `css-class` | `string` | "flex gap-2 items-center" | CSS class for container |
| `js-mode` | `JavaScriptMode?` | null | JavaScript framework mode |
| `use-htmx` | `bool` | true | Enable HTMX (deprecated) |
| `htmx-target` | `string` | "" | HTMX target element ID |
| `id` | `string?` | auto-generated | Custom ID for the pager |

### Advanced

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `enable-token-accumulation` | `bool` | true | Store token history for backward navigation |
| `search-term` | `string?` | null | Search term to preserve |
| `link-url` / `href` | `string?` | current path | Base URL for navigation |
| `action` | `string?` | null | ASP.NET Core MVC action |
| `controller` | `string?` | null | ASP.NET Core MVC controller |
| `language` | `string?` | null | Localization language code |

## Token Management

### Token Accumulation

The Continuation Pager can store token history to enable backward navigation:

```csharp
public class ProductsViewModel : IContinuationPagingModel
{
    public string? NextPageToken { get; set; }
    public bool HasMoreResults { get; set; }
    public int PageSize { get; set; } = 25;
    public int CurrentPage { get; set; } = 1;
    public Dictionary<int, string>? PageTokenHistory { get; set; }
    public ViewType ViewType { get; set; } = ViewType.TailwindAndDaisy;

    public List<Product> Products { get; set; } = new();
}
```

### How Token History Works

1. **Page 1**: No token needed, `PageTokenHistory` is empty
2. **Page 2**: Store token to get back to Page 1
3. **Page 3**: Store token to get back to Page 2
4. **Going Back**: Use stored token from history

### Controller Pattern

```csharp
public async Task<IActionResult> Index(
    int currentPage = 1,
    int pageSize = 25,
    string? pageToken = null,
    string? tokenHistory = null)
{
    // Deserialize token history
    var history = string.IsNullOrEmpty(tokenHistory)
        ? new Dictionary<int, string>()
        : JsonSerializer.Deserialize<Dictionary<int, string>>(tokenHistory);

    // Query database with token
    var (products, nextToken, hasMore) = await QueryWithToken(pageToken, pageSize);

    // Store current token in history for backward navigation
    if (!string.IsNullOrEmpty(pageToken) && history != null)
    {
        history[currentPage] = pageToken;
    }

    var viewModel = new ProductsViewModel
    {
        CurrentPage = currentPage,
        PageSize = pageSize,
        NextPageToken = nextToken,
        HasMoreResults = hasMore,
        PageTokenHistory = history,
        Products = products
    };

    return View(viewModel);
}
```

## Database-Specific Examples

### Cosmos DB

```csharp
public async Task<IActionResult> Products(
    int pageSize = 25,
    string? continuationToken = null,
    int currentPage = 1,
    string? tokenHistory = null)
{
    var query = _container.GetItemQueryIterator<Product>(
        requestOptions: new QueryRequestOptions
        {
            MaxItemCount = pageSize
        },
        continuationToken: continuationToken);

    var response = await query.ReadNextAsync();

    // Parse token history
    var history = string.IsNullOrEmpty(tokenHistory)
        ? new Dictionary<int, string>()
        : JsonSerializer.Deserialize<Dictionary<int, string>>(tokenHistory);

    // Store current token
    if (!string.IsNullOrEmpty(continuationToken))
    {
        history![currentPage] = continuationToken;
    }

    var viewModel = new ProductsViewModel
    {
        CurrentPage = currentPage,
        PageSize = pageSize,
        NextPageToken = response.ContinuationToken,
        HasMoreResults = !string.IsNullOrEmpty(response.ContinuationToken),
        PageTokenHistory = history,
        Products = response.ToList()
    };

    return View(viewModel);
}
```

```html
@model ProductsViewModel

<div id="products">
    @foreach (var product in Model.Products)
    {
        <div class="product-card">
            <h3>@product.Name</h3>
            <p>@product.Description</p>
            <p class="price">@product.Price.ToString("C")</p>
        </div>
    }
</div>

<continuation-pager
    model="Model"
    htmx-target="#products" />
```

### Azure Table Storage

```csharp
public async Task<IActionResult> Logs(
    int pageSize = 100,
    string? nextPartitionKey = null,
    string? nextRowKey = null,
    int currentPage = 1,
    string? tokenHistory = null)
{
    var query = new TableQuery<LogEntity>().Take(pageSize);

    TableContinuationToken? token = null;
    if (!string.IsNullOrEmpty(nextPartitionKey) && !string.IsNullOrEmpty(nextRowKey))
    {
        token = new TableContinuationToken
        {
            NextPartitionKey = nextPartitionKey,
            NextRowKey = nextRowKey
        };
    }

    var result = await _table.ExecuteQuerySegmentedAsync(query, token);

    // Serialize continuation token
    string? nextToken = null;
    if (result.ContinuationToken != null)
    {
        nextToken = JsonSerializer.Serialize(new
        {
            result.ContinuationToken.NextPartitionKey,
            result.ContinuationToken.NextRowKey
        });
    }

    var history = string.IsNullOrEmpty(tokenHistory)
        ? new Dictionary<int, string>()
        : JsonSerializer.Deserialize<Dictionary<int, string>>(tokenHistory);

    if (token != null)
    {
        history![currentPage] = JsonSerializer.Serialize(new
        {
            token.NextPartitionKey,
            token.NextRowKey
        });
    }

    var viewModel = new LogsViewModel
    {
        CurrentPage = currentPage,
        PageSize = pageSize,
        NextPageToken = nextToken,
        HasMoreResults = result.ContinuationToken != null,
        PageTokenHistory = history,
        Logs = result.Results.ToList()
    };

    return View(viewModel);
}
```

### AWS DynamoDB

```csharp
public async Task<IActionResult> Items(
    int pageSize = 50,
    string? lastEvaluatedKey = null,
    int currentPage = 1,
    string? tokenHistory = null)
{
    var request = new ScanRequest
    {
        TableName = "Items",
        Limit = pageSize,
        ExclusiveStartKey = string.IsNullOrEmpty(lastEvaluatedKey)
            ? null
            : JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(lastEvaluatedKey)
    };

    var response = await _dynamoClient.ScanAsync(request);

    // Serialize LastEvaluatedKey
    string? nextToken = response.LastEvaluatedKey?.Count > 0
        ? JsonSerializer.Serialize(response.LastEvaluatedKey)
        : null;

    var history = string.IsNullOrEmpty(tokenHistory)
        ? new Dictionary<int, string>()
        : JsonSerializer.Deserialize<Dictionary<int, string>>(tokenHistory);

    if (!string.IsNullOrEmpty(lastEvaluatedKey))
    {
        history![currentPage] = lastEvaluatedKey;
    }

    var viewModel = new ItemsViewModel
    {
        CurrentPage = currentPage,
        PageSize = pageSize,
        NextPageToken = nextToken,
        HasMoreResults = !string.IsNullOrEmpty(nextToken),
        PageTokenHistory = history,
        Items = response.Items.Select(MapToItem).ToList()
    };

    return View(viewModel);
}
```

### MongoDB Cursor Pagination

```csharp
public async Task<IActionResult> Documents(
    int pageSize = 20,
    string? cursorId = null,
    int currentPage = 1,
    string? tokenHistory = null)
{
    var filter = Builders<Document>.Filter.Empty;

    // Apply cursor filter
    if (!string.IsNullOrEmpty(cursorId))
    {
        filter = Builders<Document>.Filter.Gt(d => d.Id, cursorId);
    }

    var documents = await _collection
        .Find(filter)
        .Sort(Builders<Document>.Sort.Ascending(d => d.Id))
        .Limit(pageSize + 1) // Get one extra to check for more results
        .ToListAsync();

    var hasMore = documents.Count > pageSize;
    if (hasMore)
    {
        documents = documents.Take(pageSize).ToList();
    }

    var nextToken = hasMore ? documents.Last().Id : null;

    var history = string.IsNullOrEmpty(tokenHistory)
        ? new Dictionary<int, string>()
        : JsonSerializer.Deserialize<Dictionary<int, string>>(tokenHistory);

    if (!string.IsNullOrEmpty(cursorId))
    {
        history![currentPage] = cursorId;
    }

    var viewModel = new DocumentsViewModel
    {
        CurrentPage = currentPage,
        PageSize = pageSize,
        NextPageToken = nextToken,
        HasMoreResults = hasMore,
        PageTokenHistory = history,
        Documents = documents
    };

    return View(viewModel);
}
```

## JavaScript Modes

### HTMX (Default)

```html
<continuation-pager
    model="Model"
    js-mode="HTMX"
    htmx-target="#content" />
```

### Plain JavaScript

```html
<continuation-pager
    model="Model"
    js-mode="PlainJS" />
```

### No JavaScript

Uses standard links:

```html
<continuation-pager
    model="Model"
    js-mode="NoJS" />
```

## Styling Options

### TailwindCSS + DaisyUI

```html
<continuation-pager
    model="Model"
    view-type="TailwindAndDaisy" />
```

### Pure TailwindCSS

```html
<continuation-pager
    model="Model"
    view-type="Tailwind" />
```

### Bootstrap

```html
<continuation-pager
    model="Model"
    view-type="Bootstrap" />
```

### Plain CSS

```html
<continuation-pager
    model="Model"
    view-type="Plain" />
```

## Advanced Features

### Hide Page Number

```html
<continuation-pager
    model="Model"
    show-page-number="false" />
```

### Hide Page Size Selector

```html
<continuation-pager
    model="Model"
    show-pagesize="false" />
```

### Custom Button Text

```html
<continuation-pager
    model="Model"
    previous-page-text="Back"
    next-page-text="Forward" />
```

### Disable Token Accumulation

If you don't need backward navigation:

```html
<continuation-pager
    model="Model"
    enable-token-accumulation="false" />
```

### With Search

```html
<form method="get">
    <input type="text" name="search" value="@Model.SearchTerm" />
    <button type="submit">Search</button>
</form>

<div id="results">
    @* Results *@
</div>

<continuation-pager
    model="Model"
    search-term="@Model.SearchTerm"
    htmx-target="#results" />
```

## Complete Example

### ViewModel

```csharp
public class ProductsViewModel : IContinuationPagingModel
{
    public string? NextPageToken { get; set; }
    public bool HasMoreResults { get; set; }
    public int PageSize { get; set; } = 25;
    public int CurrentPage { get; set; } = 1;
    public Dictionary<int, string>? PageTokenHistory { get; set; }
    public ViewType ViewType { get; set; } = ViewType.TailwindAndDaisy;
    public string? SearchTerm { get; set; }

    public List<Product> Products { get; set; } = new();
}
```

### Controller

```csharp
public async Task<IActionResult> Index(
    int currentPage = 1,
    int pageSize = 25,
    string? pageToken = null,
    string? search = null,
    string? tokenHistory = null)
{
    // Deserialize token history
    var history = string.IsNullOrEmpty(tokenHistory)
        ? new Dictionary<int, string>()
        : JsonSerializer.Deserialize<Dictionary<int, string>>(tokenHistory);

    // Build query with continuation token
    var queryDefinition = new QueryDefinition(
        "SELECT * FROM c WHERE CONTAINS(c.name, @search) ORDER BY c.name"
    ).WithParameter("@search", search ?? "");

    var query = _container.GetItemQueryIterator<Product>(
        queryDefinition,
        continuationToken: pageToken,
        requestOptions: new QueryRequestOptions { MaxItemCount = pageSize }
    );

    var response = await query.ReadNextAsync();

    // Store current token in history
    if (!string.IsNullOrEmpty(pageToken))
    {
        history[currentPage] = pageToken;
    }

    var viewModel = new ProductsViewModel
    {
        CurrentPage = currentPage,
        PageSize = pageSize,
        SearchTerm = search,
        NextPageToken = response.ContinuationToken,
        HasMoreResults = !string.IsNullOrEmpty(response.ContinuationToken),
        PageTokenHistory = history,
        Products = response.ToList()
    };

    return View(viewModel);
}
```

### View

```html
@model ProductsViewModel

<div class="container">
    <h1>Products</h1>

    <form method="get" class="search-form">
        <input type="text"
               name="search"
               value="@Model.SearchTerm"
               placeholder="Search products..." />
        <button type="submit">Search</button>
    </form>

    <div id="products-container">
        <div class="products-grid">
            @foreach (var product in Model.Products)
            {
                <div class="product-card">
                    <h3>@product.Name</h3>
                    <p>@product.Description</p>
                    <p class="price">@product.Price.ToString("C")</p>
                    <button>Add to Cart</button>
                </div>
            }
        </div>

        @if (Model.Products.Count == 0)
        {
            <p>No products found.</p>
        }
    </div>

    <continuation-pager
        model="Model"
        htmx-target="#products-container"
        show-page-number="true"
        show-pagesize="true" />
</div>
```

## Comparison with Traditional Pager

| Feature | Continuation Pager | Traditional Pager |
|---------|-------------------|-------------------|
| **Navigation** | Previous/Next only | First/Previous/Page Numbers/Next/Last |
| **Use Case** | NoSQL databases, continuation tokens | SQL databases, offset/limit |
| **Page Jumping** | Not possible | Can jump to any page |
| **Performance** | Excellent for large datasets | Can be slow with large offsets |
| **Token Storage** | Required for backward navigation | Not needed |
| **Best For** | Cosmos DB, DynamoDB, Table Storage | SQL Server, PostgreSQL, MySQL |

## See Also

- [Pager TagHelper](pager-taghelper.md) - Traditional page number-based pagination
- [PageSize TagHelper](pagesize-taghelper.md) - Standalone page size selector
- [JavaScript Modes Guide](javascript-modes.md) - Detailed guide on JavaScript modes
- [Custom Views Guide](custom-views.md) - Creating your own views
