# PageSize TagHelper

The `<page-size>` TagHelper provides a standalone page size selector control. Unlike the full Pager TagHelper, this component focuses solely on allowing users to change the number of items displayed per page.

## Table of Contents

- [When to Use PageSize TagHelper](#when-to-use-pagesize-taghelper)
- [Basic Usage](#basic-usage)
- [Attributes Reference](#attributes-reference)
- [Use Cases](#use-cases)
- [JavaScript Modes](#javascript-modes)
- [Styling Options](#styling-options)
- [Examples](#examples)

## When to Use PageSize TagHelper

Use the PageSize TagHelper when:

1. **Cosmos DB / NoSQL Databases**: When using continuation tokens where traditional page numbers don't exist
2. **Infinite Scroll**: When implementing infinite scroll but want to control batch size
3. **Standalone Control**: When you need only page size selection without full pagination
4. **Custom Layouts**: When building custom pagination layouts where page size needs separate placement
5. **Performance Tuning**: When you want users to control result set size for performance reasons

## Basic Usage

### Standalone Usage

```html
<page-size
    page-size="@Model.PageSize"
    total-items="@Model.TotalItems"
    link-url="/Products" />
```

### With IPagingModel

```html
<page-size model="Model" />
```

## Attributes Reference

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `model` | `IPagingModel?` | null | The paging model containing pagination details |
| `page-size` | `int?` | 10 | Current number of items per page |
| `total-items` | `int?` | 0 | Total number of items in the dataset |
| `max-page-size` | `int` | 1000 | Maximum allowed page size in dropdown |
| `page-size-steps` | `string?` | "10,25,50,75,100,125,150,200,250,500,1000" | Comma-separated list of page sizes |
| `id` | `string?` | auto-generated | Custom ID for the control |
| `view-type` | `ViewType` | TailwindAndDaisy | UI framework: TailwindAndDaisy, Tailwind, Bootstrap, Plain, NoJS |
| `use-local-view` | `bool` | false | Use a custom view from your application |
| `js-mode` | `JavaScriptMode?` | null | JavaScript framework: HTMX, HTMXWithAlpine, Alpine, PlainJS, NoJS |
| `use-htmx` | `bool` | true | Enable HTMX (deprecated, use js-mode instead) |
| `link-url` / `href` | `string?` | current path | Hard-coded link URL |
| `action` | `string?` | null | ASP.NET Core MVC action name |
| `controller` | `string?` | null | ASP.NET Core MVC controller name |
| `language` | `string?` | null | Language/culture code (e.g., "en", "fr", "de") |

## Use Cases

### Use Case 1: Cosmos DB with Continuation Tokens

When working with Cosmos DB, you don't have traditional page numbers, but you still want users to control batch size:

```csharp
public class ProductsViewModel
{
    public int PageSize { get; set; } = 25;
    public int TotalItems { get; set; }
    public string? ContinuationToken { get; set; }
    public List<Product> Products { get; set; } = new();
}
```

```html
@model ProductsViewModel

<div>
    <h2>Products (Showing @Model.PageSize items)</h2>
    <page-size
        page-size="@Model.PageSize"
        total-items="@Model.TotalItems"
        link-url="/Products" />
</div>

<div id="products">
    @foreach (var product in Model.Products)
    {
        <div>@product.Name</div>
    }
</div>

@if (!string.IsNullOrEmpty(Model.ContinuationToken))
{
    <a href="/Products?token=@Model.ContinuationToken&pageSize=@Model.PageSize">
        Load More
    </a>
}
```

### Use Case 2: Infinite Scroll with Batch Size Control

```html
@model ProductsViewModel

<div class="controls">
    <label>Items per batch:</label>
    <page-size
        page-size="@Model.PageSize"
        total-items="@Model.TotalItems"
        page-size-steps="10,20,50,100"
        js-mode="HTMX" />
</div>

<div id="products-container"
     hx-get="/Products/LoadMore"
     hx-trigger="revealed"
     hx-swap="afterbegin">
    @foreach (var product in Model.Products)
    {
        <div class="product">@product.Name</div>
    }
</div>
```

### Use Case 3: Custom Pagination Layout

```html
<div class="custom-pagination">
    <div class="left">
        <page-size model="Model" />
    </div>

    <div class="center">
        <!-- Custom page navigation -->
        <button onclick="goToPage(1)">First</button>
        <button onclick="goToPage(@(Model.Page - 1))">Previous</button>
        <span>Page @Model.Page of @Model.TotalPages</span>
        <button onclick="goToPage(@(Model.Page + 1))">Next</button>
        <button onclick="goToPage(@Model.TotalPages)">Last</button>
    </div>

    <div class="right">
        <span>Total: @Model.TotalItems items</span>
    </div>
</div>
```

### Use Case 4: Azure Table Storage

When using Azure Table Storage with continuation tokens:

```csharp
public class LogsViewModel
{
    public int PageSize { get; set; } = 100;
    public int TotalItems { get; set; }
    public TableContinuationToken? ContinuationToken { get; set; }
    public List<LogEntry> Logs { get; set; } = new();
}
```

```html
@model LogsViewModel

<div class="logs-header">
    <h2>System Logs</h2>
    <page-size
        page-size="@Model.PageSize"
        total-items="@Model.TotalItems"
        page-size-steps="50,100,200,500"
        max-page-size="500" />
</div>

<table>
    @foreach (var log in Model.Logs)
    {
        <tr>
            <td>@log.Timestamp</td>
            <td>@log.Level</td>
            <td>@log.Message</td>
        </tr>
    }
</table>
```

## Custom Page Size Steps

You can customize which page sizes appear in the dropdown:

### Simple Steps

```html
<page-size
    page-size="@Model.PageSize"
    total-items="@Model.TotalItems"
    page-size-steps="5,10,25,50" />
```

### Large Datasets

```html
<page-size
    page-size="@Model.PageSize"
    total-items="@Model.TotalItems"
    page-size-steps="100,250,500,1000,2500,5000"
    max-page-size="5000" />
```

### Performance-Oriented

```html
<page-size
    page-size="@Model.PageSize"
    total-items="@Model.TotalItems"
    page-size-steps="10,25,50"
    max-page-size="50" />
```

## JavaScript Modes

### HTMX (Default)

The control refreshes content without full page reload:

```html
<page-size
    model="Model"
    js-mode="HTMX" />
```

### HTMX with Target

Specify where to update content:

```html
<div id="product-list">
    @* Products here *@
</div>

<page-size
    model="Model"
    js-mode="HTMX"
    hx-target="#product-list" />
```

### Plain JavaScript

For applications without HTMX:

```html
<page-size
    model="Model"
    js-mode="PlainJS" />
```

You must include the plain JavaScript snippet:

```html
@Html.PageSizeOnchangeSnippet()
```

### No JavaScript

Uses a form with submit button:

```html
<page-size
    model="Model"
    js-mode="NoJS" />
```

This renders:
```html
<form method="get" action="/Products">
    <label>Page size:</label>
    <select name="pageSize">
        <option value="10">10</option>
        <option value="25">25</option>
        <option value="50">50</option>
    </select>
    <button type="submit">Apply</button>
</form>
```

## Styling Options

### TailwindCSS + DaisyUI

```html
<page-size model="Model" view-type="TailwindAndDaisy" />
```

### Pure TailwindCSS

```html
<page-size model="Model" view-type="Tailwind" />
```

### Bootstrap

```html
<page-size model="Model" view-type="Bootstrap" />
```

### Plain CSS

```html
<page-size model="Model" view-type="Plain" />

@* Include CSS in layout *@
@Html.PlainCSS()
```

## Localization

The label text is automatically localized based on the `language` attribute:

```html
<!-- French -->
<page-size model="Model" language="fr" />
<!-- Renders: "Taille de la page:" -->

<!-- German -->
<page-size model="Model" language="de" />
<!-- Renders: "Seitengröße:" -->

<!-- Spanish -->
<page-size model="Model" language="es" />
<!-- Renders: "Tamaño de página:" -->
```

Supported languages:
- English (en)
- French (fr)
- German (de)
- Spanish (es)
- Italian (it)
- Portuguese (pt)
- Japanese (ja)
- Chinese Simplified (zh-Hans)

## Examples

### Example 1: Basic Standalone Control

```html
<page-size
    page-size="25"
    total-items="1000"
    link-url="/api/products" />
```

### Example 2: With Custom Steps

```html
<page-size
    page-size="@Model.PageSize"
    total-items="@Model.TotalItems"
    page-size-steps="10,20,50,100,200"
    max-page-size="200" />
```

### Example 3: Cosmos DB Scenario

```csharp
[HttpGet]
public async Task<IActionResult> Products(int pageSize = 25, string? continuationToken = null)
{
    var query = _container
        .GetItemQueryIterator<Product>(
            requestOptions: new QueryRequestOptions
            {
                MaxItemCount = pageSize
            });

    var response = await query.ReadNextAsync();

    var viewModel = new ProductsViewModel
    {
        PageSize = pageSize,
        Products = response.ToList(),
        ContinuationToken = response.ContinuationToken,
        TotalItems = await _container.GetItemLinqQueryable<Product>()
            .CountAsync()
    };

    return View(viewModel);
}
```

```html
@model ProductsViewModel

<div class="header">
    <h1>Products</h1>
    <page-size
        page-size="@Model.PageSize"
        total-items="@Model.TotalItems"
        page-size-steps="10,25,50,100" />
</div>

<div id="products">
    @foreach (var product in Model.Products)
    {
        <div>@product.Name - @product.Price</div>
    }
</div>

@if (!string.IsNullOrEmpty(Model.ContinuationToken))
{
    <a href="?pageSize=@Model.PageSize&continuationToken=@Uri.EscapeDataString(Model.ContinuationToken)">
        Load More
    </a>
}
```

### Example 4: DynamoDB with Scan Limit

```csharp
public async Task<IActionResult> Items(int pageSize = 50, string? lastEvaluatedKey = null)
{
    var request = new ScanRequest
    {
        TableName = "Items",
        Limit = pageSize,
        ExclusiveStartKey = ParseLastKey(lastEvaluatedKey)
    };

    var response = await _dynamoClient.ScanAsync(request);

    var viewModel = new ItemsViewModel
    {
        PageSize = pageSize,
        Items = response.Items.Select(MapToItem).ToList(),
        LastEvaluatedKey = SerializeLastKey(response.LastEvaluatedKey),
        TotalItems = response.ScannedCount
    };

    return View(viewModel);
}
```

```html
@model ItemsViewModel

<page-size
    page-size="@Model.PageSize"
    total-items="@Model.TotalItems"
    page-size-steps="25,50,100,250"
    js-mode="HTMX" />

<div id="items">
    @foreach (var item in Model.Items)
    {
        <div>@item.Name</div>
    }
</div>
```

### Example 5: Elasticsearch with Size Parameter

```csharp
public async Task<IActionResult> Search(string query, int pageSize = 20, int from = 0)
{
    var searchResponse = await _elasticClient.SearchAsync<Document>(s => s
        .Query(q => q.Match(m => m.Field(f => f.Content).Query(query)))
        .Size(pageSize)
        .From(from));

    var viewModel = new SearchViewModel
    {
        PageSize = pageSize,
        Results = searchResponse.Documents.ToList(),
        TotalItems = (int)searchResponse.Total,
        From = from
    };

    return View(viewModel);
}
```

```html
@model SearchViewModel

<div class="search-controls">
    <input type="text" name="query" value="@Model.Query" />
    <page-size
        page-size="@Model.PageSize"
        total-items="@Model.TotalItems"
        page-size-steps="10,20,50,100" />
</div>
```

### Example 6: Without Page Numbers (Pure Batch Size)

Perfect for scenarios where you don't track total pages:

```html
<div class="batch-control">
    <label>Results per batch:</label>
    <page-size
        page-size="@Model.PageSize"
        total-items="@Model.TotalItems"
        page-size-steps="25,50,100"
        js-mode="HTMX" />
</div>

<button hx-get="/api/next-batch"
        hx-target="#results"
        hx-vals='{"pageSize": "@Model.PageSize"}'>
    Load Next Batch
</button>
```

### Example 7: With Localization

```html
<!-- Automatic based on current culture -->
<page-size model="Model" />

<!-- Explicit French -->
<page-size model="Model" language="fr" />

<!-- Explicit Japanese -->
<page-size model="Model" language="ja" />
```

## Integration with Pager TagHelper

The PageSize TagHelper is automatically included when using the full Pager TagHelper:

```html
<!-- Includes page size selector -->
<paging model="Model" />

<!-- Hides page size selector -->
<paging model="Model" show-pagesize="false" />
```

## Performance Considerations

### Limiting Maximum Page Size

Protect your database by limiting maximum page size:

```html
<page-size
    model="Model"
    max-page-size="100" />
```

### Database-Specific Recommendations

- **Cosmos DB**: 25-100 items per request
- **Azure Table Storage**: 100-1000 items per request
- **SQL Server**: 10-50 items for complex queries, 100+ for simple queries
- **Elasticsearch**: 10-100 items per request
- **DynamoDB**: 50-100 items per scan

## See Also

- [Pager TagHelper](pager-taghelper.md) - Full pagination with page numbers
- [Continuation Pager TagHelper](continuation-pager-taghelper.md) - For continuation token-based pagination
- [JavaScript Modes Guide](javascript-modes.md) - Detailed guide on JavaScript modes
- [Custom Views Guide](custom-views.md) - Creating your own views
