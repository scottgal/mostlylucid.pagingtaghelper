# API Reference

Complete reference documentation for all TagHelpers, interfaces, and classes in the Paging TagHelper library.

## Table of Contents

- [Pager TagHelper](#pager-taghelper)
- [PageSize TagHelper](#pagesize-taghelper)
- [SortableHeader TagHelper](#sortableheader-taghelper)
- [ContinuationPager TagHelper](#continuationpager-taghelper)
- [Interfaces](#interfaces)
- [Enums](#enums)
- [View Models](#view-models)
- [Extension Methods](#extension-methods)

## Pager TagHelper

**Tag:** `<paging>`

Full-featured pagination control with page navigation, page size selection, and summary display.

### Attributes

#### Core Pagination

| Attribute | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `model` | `IPagingModel?` | null | No* | Paging model containing pagination data |
| `page` | `int?` | null | No* | Current page number (1-based) |
| `page-size` | `int?` | null | No* | Number of items per page |
| `total-items` | `int?` | null | No* | Total number of items in dataset |

*Either `model` OR all of `page`, `page-size`, and `total-items` must be provided.

#### Display Control

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `show-pagesize` | `bool` | `true` | Show page size dropdown selector |
| `show-summary` | `bool` | `true` | Show pagination summary text |
| `first-last-navigation` | `bool` | `true` | Show first/last page buttons |
| `skip-forward-back-navigation` | `bool` | `true` | Show skip forward/backward ellipsis |
| `pages-to-display` | `int` | `5` | Number of page buttons to show |
| `id` | `string?` | auto-generated | HTML ID attribute for the pager container |

#### Styling

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `view-type` | `ViewType` | `TailwindAndDaisy` | UI framework/style to use |
| `css-class` | `string` | `"btn-group"` | CSS class for pager container |
| `use-local-view` | `bool` | `false` | Use custom view from application |

**ViewType Options:**
- `TailwindAndDaisy` - TailwindCSS + DaisyUI components
- `Tailwind` - Pure TailwindCSS utilities
- `Bootstrap` - Bootstrap 5 styling
- `Plain` - Embedded plain CSS
- `NoJS` - Zero-JavaScript with forms
- `Custom` - Your custom view

#### JavaScript Modes

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `js-mode` | `JavaScriptMode?` | `null` | JavaScript framework mode |
| `use-htmx` | `bool` | `true` | **Deprecated.** Use `js-mode` instead |
| `htmx-target` / `hx-target` | `string?` | `""` | HTMX target element selector |
| `hx-swap` | `string?` | `null` | HTMX swap strategy |
| `hx-push-url` | `string?` | `null` | HTMX browser history push |
| `hx-indicator` | `string?` | `null` | HTMX loading indicator selector |

**JavaScriptMode Options:**
- `HTMX` - HTMX AJAX navigation
- `HTMXWithAlpine` - HTMX + Alpine.js
- `Alpine` - Alpine.js only
- `PlainJS` - Vanilla JavaScript
- `NoJS` - No JavaScript

#### Text Customization

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `first-page-text` | `string?` | `"«"` | First page button text |
| `previous-page-text` | `string?` | `"‹ Previous"` | Previous page button text |
| `next-page-text` | `string?` | `"Next ›"` | Next page button text |
| `last-page-text` | `string?` | `"»"` | Last page button text |
| `skip-back-text` | `string?` | `".."` | Skip backward ellipsis text |
| `skip-forward-text` | `string?` | `".."` | Skip forward ellipsis text |
| `next-page-aria-label` | `string?` | `"go to next page"` | ARIA label for next button |
| `previous-page-aria-label` | `string?` | `"go to previous page"` | ARIA label for previous button |

#### Localization

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `language` | `string?` | `null` | Language/culture code (e.g., "en", "fr", "de") |
| `summary-template` | `string?` | `null` | Custom summary template with placeholders |

**Supported Languages:** `en`, `fr`, `de`, `es`, `it`, `pt`, `ja`, `zh-Hans`

**Template Placeholders:** `{currentPage}`, `{totalPages}`, `{totalItems}`, `{pageSize}`, `{startItem}`, `{endItem}`

#### Sorting

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `order-by` | `string?` | `null` | Column name for sorting |
| `descending` | `bool?` | `null` | Sort in descending order |

#### URL Configuration

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `link-url` / `href` | `string?` | current path | Base URL for pagination links |
| `action` | `string?` | `null` | ASP.NET Core MVC action name |
| `controller` | `string?` | `null` | ASP.NET Core MVC controller name |
| `area` | `string?` | `null` | ASP.NET Core MVC area name |
| `search-term` | `string?` | `null` | Search term to preserve |

#### Page Size Configuration

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `page-size-steps` | `string?` | `null` | Comma-separated page size options (e.g., "10,25,50") |
| `max-page-size` | `int` | `1000` | Maximum allowed page size |

### Examples

```html
<!-- Minimal -->
<paging model="Model" />

<!-- With HTMX -->
<paging model="Model" htmx-target="#results" />

<!-- Fully customized -->
<paging model="Model"
        view-type="Tailwind"
        js-mode="HTMX"
        language="fr"
        show-pagesize="true"
        first-page-text="⏮ First"
        last-page-text="Last ⏭"
        summary-template="Showing {startItem}-{endItem} of {totalItems}"
        htmx-target="#content"
        hx-swap="outerHTML"
        hx-push-url="true" />
```

---

## PageSize TagHelper

**Tag:** `<page-size>`

Standalone page size selector dropdown.

### Attributes

| Attribute | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `model` | `IPagingModel?` | null | No* | Paging model |
| `page-size` | `int?` | null | No* | Current page size |
| `total-items` | `int?` | null | No* | Total items |
| `page-size-steps` | `string?` | `null` | No | Comma-separated size options |
| `link-url` / `href` | `string?` | current path | No | Base URL for navigation |
| `use-htmx` | `bool` | `true` | No | Enable HTMX mode |
| `js-mode` | `JavaScriptMode?` | `null` | No | JavaScript framework mode |
| `hx-target` / `htmx-target` | `string?` | `""` | No | HTMX target selector |
| `id` | `string?` | auto-generated | No | HTML ID attribute |
| `language` | `string?` | `null` | No | Language/culture code |

*Either `model` OR both `page-size` and `total-items` must be provided.

### Examples

```html
<!-- Basic -->
<page-size model="Model" />

<!-- Custom sizes -->
<page-size model="Model" page-size-steps="5,10,25,50,100" />

<!-- With HTMX -->
<page-size model="Model"
           js-mode="HTMX"
           hx-target="#results"
           hx-swap="outerHTML" />

<!-- Standalone without model -->
<page-size page-size="25"
           total-items="500"
           link-url="/Products" />
```

---

## SortableHeader TagHelper

**Tag:** `<sortable-header>`

Clickable table header with sort direction indicator.

### Attributes

| Attribute | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `column` | `string` | - | **Yes** | Column name for sorting |
| `current-order-by` | `string?` | `null` | No | Currently sorted column |
| `descending` | `bool` | `false` | No | Currently sorting descending |
| `link-url` / `href` | `string?` | current path | No | Base URL for sort links |
| `use-htmx` | `bool` | `true` | No | Enable HTMX mode |
| `js-mode` | `JavaScriptMode?` | `null` | No | JavaScript framework mode |
| `hx-target` / `htmx-target` | `string?` | `""` | No | HTMX target selector |
| `hx-swap` | `string?` | `null` | No | HTMX swap strategy |
| `ascending-icon` | `string` | `"▲"` | No | Icon for ascending sort |
| `descending-icon` | `string` | `"▼"` | No | Icon for descending sort |
| `css-class` | `string?` | `null` | No | Custom CSS class |

### Examples

```html
<table>
    <thead>
        <tr>
            <sortable-header column="Name"
                           current-order-by="@Model.OrderBy"
                           descending="@Model.Descending">
                Product Name
            </sortable-header>

            <sortable-header column="Price"
                           current-order-by="@Model.OrderBy"
                           descending="@Model.Descending"
                           hx-target="#product-table">
                Price
            </sortable-header>
        </tr>
    </thead>
</table>
```

---

## ContinuationPager TagHelper

**Tag:** `<continuation-pager>`

Pagination for NoSQL databases using continuation tokens (Cosmos DB, DynamoDB, etc.).

### Attributes

| Attribute | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `model` | `IContinuationPagingModel` | - | **Yes** | Continuation paging model |
| `show-page-number` | `bool` | `true` | No | Display page numbers |
| `show-pagesize` | `bool` | `true` | No | Show page size selector |
| `enable-token-accumulation` | `bool` | `true` | No | Store token history for backward navigation |
| `max-history-pages` | `int` | `20` | No | Maximum pages to store in token history |
| `preserve-query-parameters` | `bool` | `true` | No | Preserve URL query parameters |
| `link-url` / `href` | `string?` | current path | No | Base URL |
| `htmx-target` / `hx-target` | `string?` | `""` | No | HTMX target selector |
| `js-mode` | `JavaScriptMode?` | `null` | No | JavaScript framework mode |
| `previous-page-text` | `string?` | `"‹ Previous"` | No | Previous button text |
| `next-page-text` | `string?` | `"Next ›"` | No | Next button text |
| `view-type` | `ViewType` | `TailwindAndDaisy` | No | UI framework style |
| `id` | `string?` | auto-generated | No | HTML ID attribute |
| `language` | `string?` | `null` | No | Language/culture code |

### Examples

```html
<!-- Basic -->
<continuation-pager model="Model" />

<!-- With HTMX and numbered pages -->
<continuation-pager model="Model"
                   htmx-target="#results"
                   show-page-number="true"
                   max-history-pages="30" />

<!-- Minimal (no page numbers) -->
<continuation-pager model="Model"
                   show-page-number="false"
                   show-pagesize="false" />
```

---

## Interfaces

### IPagingModel

Base interface for pagination models.

```csharp
public interface IPagingModel
{
    int Page { get; set; }
    int PageSize { get; set; }
    int TotalItems { get; set; }
    ViewType ViewType { get; set; }
    string? LinkUrl { get; set; }
}
```

**Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `Page` | `int` | Current page number (1-based) |
| `PageSize` | `int` | Number of items per page |
| `TotalItems` | `int` | Total number of items in dataset |
| `ViewType` | `ViewType` | UI framework style |
| `LinkUrl` | `string?` | Base URL for pagination links |

### IPagingModel<T>

Generic interface that extends `IPagingModel` with typed data collection.

```csharp
public interface IPagingModel<T> : IPagingModel
{
    List<T> Items { get; set; }
}
```

**Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `Items` | `List<T>` | Collection of items for current page |

### IContinuationPagingModel

Interface for continuation token-based pagination.

```csharp
public interface IContinuationPagingModel
{
    string? NextPageToken { get; set; }
    bool HasMoreResults { get; set; }
    int PageSize { get; set; }
    int CurrentPage { get; set; }
    Dictionary<int, string>? PageTokenHistory { get; set; }
}
```

**Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `NextPageToken` | `string?` | Token for next page from database |
| `HasMoreResults` | `bool` | Whether more results are available |
| `PageSize` | `int` | Number of items per page |
| `CurrentPage` | `int` | Current page number |
| `PageTokenHistory` | `Dictionary<int, string>?` | Map of page numbers to continuation tokens |

### IPagingLocalizer

Interface for localization support.

```csharp
public interface IPagingLocalizer
{
    string FirstPageText { get; }
    string PreviousPageText { get; }
    string NextPageText { get; }
    string LastPageText { get; }
    string SkipForwardText { get; }
    string SkipBackText { get; }
    string PageSizeLabel { get; }
    string NextPageAriaLabel { get; }
    string PreviousPageAriaLabel { get; }
    string GetPageSummary(int currentPage, int totalPages, int totalItems);
}
```

---

## Enums

### ViewType

Determines which UI framework/style to use.

```csharp
public enum ViewType
{
    TailwindAndDaisy,  // TailwindCSS + DaisyUI components
    Tailwind,          // Pure TailwindCSS utilities
    Bootstrap,         // Bootstrap 5 styling
    Plain,             // Embedded plain CSS
    NoJS,              // Zero-JavaScript with forms
    Custom             // Custom view from application
}
```

### JavaScriptMode

Determines which JavaScript framework to use.

```csharp
public enum JavaScriptMode
{
    HTMX,              // HTMX for AJAX navigation
    HTMXWithAlpine,    // HTMX + Alpine.js
    Alpine,            // Alpine.js only
    PlainJS,           // Vanilla JavaScript
    NoJS               // No JavaScript
}
```

---

## View Models

### PagerViewModel

View model passed to Pager views.

```csharp
public class PagerViewModel : BaseTagModel
{
    // Pagination
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; }
    public int StartPage { get; }
    public int EndPage { get; }

    // Display Control
    public bool ShowPageSize { get; set; }
    public bool ShowSummary { get; set; }
    public bool FirstLastNavigation { get; set; }
    public bool SkipForwardBackNavigation { get; set; }
    public int PagesToDisplay { get; set; }

    // Text/Labels
    public string FirstPageText { get; set; }
    public string PreviousPageText { get; set; }
    public string NextPageText { get; set; }
    public string LastPageText { get; set; }
    public string SkipBackText { get; set; }
    public string SkipForwardText { get; set; }
    public string NextPageAriaLabel { get; set; }
    public string PreviousPageAriaLabel { get; set; }

    // URLs and Navigation
    public string LinkUrl { get; set; }
    public string? SearchTerm { get; set; }
    public string CssClass { get; set; }
    public string HtmxTarget { get; set; }

    // Sorting
    public string? OrderBy { get; set; }
    public bool? Descending { get; set; }

    // Configuration
    public List<int> PageSizes { get; set; }
    public JavaScriptMode EffectiveJSMode { get; }
    public IPagingLocalizer? Localizer { get; set; }
    public string? SummaryTemplate { get; set; }

    // Methods
    public string GetPageSummary();
}
```

### PageSizeModel

View model passed to PageSize views.

```csharp
public class PageSizeModel
{
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public List<int> PageSizes { get; set; }
    public string? LinkUrl { get; set; }
    public bool UseHtmx { get; set; }
    public JavaScriptMode EffectiveJSMode { get; }
    public IPagingLocalizer? Localizer { get; set; }
    public string HtmxTarget { get; set; }
}
```

### ContinuationPagerViewModel

View model for continuation token pagers.

```csharp
public class ContinuationPagerViewModel
{
    // Token Management
    public string? NextPageToken { get; set; }
    public bool HasMoreResults { get; set; }
    public Dictionary<int, string>? PageTokenHistory { get; set; }

    // Pagination
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }

    // Display Control
    public bool ShowPageNumber { get; set; }
    public bool ShowPageSize { get; set; }
    public bool EnableTokenAccumulation { get; set; }

    // Text/Labels
    public string PreviousPageText { get; set; }
    public string NextPageText { get; set; }
    public string PreviousPageAriaLabel { get; set; }
    public string NextPageAriaLabel { get; set; }

    // URLs and Navigation
    public string LinkUrl { get; set; }
    public string CssClass { get; set; }
    public string HtmxTarget { get; set; }

    // Configuration
    public JavaScriptMode EffectiveJSMode { get; }
    public List<int> PageSizes { get; set; }

    // Methods
    public string? GetPreviousPageToken();
    public bool CanNavigatePrevious { get; }
    public bool CanNavigateNext { get; }
}
```

---

## Extension Methods

### QueryableExtensions

Helper methods for paginating `IQueryable<T>` collections.

```csharp
public static class QueryableExtensions
{
    // Paginate a queryable collection
    public static async Task<PagingResult<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagingResult<T>
        {
            Items = items,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize
        };
    }
}
```

**Usage:**
```csharp
var result = await _db.Products.ToPagedListAsync(page, pageSize);
```

### HtmlExtensions

Helper methods for embedding CSS and JavaScript.

```csharp
public static class HtmlExtensions
{
    // Inject PlainView CSS
    public static IHtmlContent PlainCSS(this IHtmlHelper htmlHelper, bool minified = true);

    // Inject page size change JavaScript snippet
    public static IHtmlContent PageSizeOnchangeSnippet(this IHtmlHelper htmlHelper);
}
```

**Usage:**
```html
<!-- In _Layout.cshtml -->
@Html.PlainCSS()

<!-- Before </body> tag -->
@Html.PageSizeOnchangeSnippet()
```

---

## Helper Classes

### PagingResult<T>

Result object returned by `ToPagedListAsync`.

```csharp
public class PagingResult<T>
{
    public List<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}
```

---

## See Also

- [Getting Started](getting-started.md)
- [Pager TagHelper](pager-taghelper.md)
- [PageSize TagHelper](pagesize-taghelper.md)
- [SortableHeader TagHelper](sortable-header-taghelper.md)
- [ContinuationPager TagHelper](continuation-pager-taghelper.md)
- [JavaScript Modes](javascript-modes.md)
- [Custom Views](custom-views.md)
- [Localization](localization.md)
- [Advanced Usage](advanced-usage.md)
