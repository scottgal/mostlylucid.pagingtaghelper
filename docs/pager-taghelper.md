# Pager TagHelper

The `<paging>` TagHelper provides comprehensive pagination controls with page navigation, page size selection, and optional sorting.

## Table of Contents

- [Basic Usage](#basic-usage)
- [Attributes Reference](#attributes-reference)
- [Using with IPagingModel](#using-with-ipagingmodel)
- [Using Individual Attributes](#using-individual-attributes)
- [JavaScript Modes](#javascript-modes)
- [Localization](#localization)
- [Custom Summary Templates](#custom-summary-templates)
- [Styling Options](#styling-options)
- [Advanced Features](#advanced-features)
- [Examples](#examples)

## Basic Usage

The simplest way to use the Pager TagHelper is with an `IPagingModel`:

```html
<paging model="Model" />
```

This renders a full-featured pagination control with:
- First/Previous/Next/Last navigation buttons
- Page numbers
- Page size selector
- Summary text (e.g., "Page 1 of 10 (Total items: 100)")

## Attributes Reference

### Core Attributes

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `model` | `IPagingModel?` | null | The paging model containing all pagination details |
| `page` | `int?` | null | Current page number (1-based) |
| `page-size` | `int?` | null | Number of items per page |
| `total-items` | `int?` | null | Total number of items in the dataset |
| `pages-to-display` | `int` | 5 | Number of page numbers to show at once |
| `id` | `string?` | auto-generated | Custom ID for the pager container |

### Display Control

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `show-pagesize` | `bool` | true | Whether to show the page size dropdown |
| `show-summary` | `bool` | true | Whether to show the pagination summary text |
| `first-last-navigation` | `bool` | true | Show first/last page navigation links |
| `skip-forward-back-navigation` | `bool` | true | Show skip forward/backward links (...) |

### Styling

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `view-type` | `ViewType` | TailwindAndDaisy | UI framework: TailwindAndDaisy, Tailwind, Bootstrap, Plain, NoJS |
| `css-class` | `string` | "btn-group" | CSS class for the pager container |
| `use-local-view` | `bool` | false | Use a custom view from your application |

### JavaScript Modes

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `js-mode` | `JavaScriptMode?` | null | JavaScript framework: HTMX, HTMXWithAlpine, Alpine, PlainJS, NoJS |
| `use-htmx` | `bool` | true | Enable HTMX (deprecated, use js-mode instead) |
| `htmx-target` | `string` | "" | HTMX target element ID for AJAX updates |

### Text Customization

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `first-page-text` | `string` | "«" | Text for the first page button |
| `previous-page-text` | `string` | "‹ Previous" | Text for the previous page button |
| `next-page-text` | `string` | "Next ›" | Text for the next page button |
| `last-page-text` | `string` | "»" | Text for the last page button |
| `skip-back-text` | `string` | ".." | Text for skip backward link |
| `skip-forward-text` | `string` | ".." | Text for skip forward link |
| `next-page-aria-label` | `string` | "go to next page" | ARIA label for next button |

### Localization

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `language` | `string?` | null | Language/culture code (e.g., "en", "fr", "de", "es", "it", "pt", "ja", "zh-Hans") |
| `summary-template` | `string?` | null | Custom template for summary text with placeholders |

### Sorting

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `order-by` | `string?` | null | Property name used for sorting |
| `descending` | `bool?` | null | Enable descending sort |

### URL Configuration

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `link-url` / `href` | `string?` | current path | Hard-coded link URL for navigation |
| `action` | `string?` | null | ASP.NET Core MVC action name |
| `controller` | `string?` | null | ASP.NET Core MVC controller name |
| `search-term` | `string?` | null | Search term to preserve across navigation |
| `max-page-size` | `int` | 1000 | Maximum allowed page size in dropdown |

## Using with IPagingModel

The recommended approach is to use `IPagingModel`:

### 1. Create Your ViewModel

```csharp
public class ProductsViewModel : IPagingModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public ViewType ViewType { get; set; } = ViewType.TailwindAndDaisy;

    public List<Product> Products { get; set; } = new();
}
```

### 2. Populate in Controller

```csharp
public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
{
    var query = _context.Products.AsQueryable();

    var viewModel = new ProductsViewModel
    {
        Page = page,
        PageSize = pageSize,
        TotalItems = await query.CountAsync(),
        Products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
    };

    return View(viewModel);
}
```

### 3. Use in View

```html
@model ProductsViewModel

<div id="products">
    @foreach (var product in Model.Products)
    {
        <div>@product.Name</div>
    }
</div>

<paging model="Model" htmx-target="#products" />
```

## Using Individual Attributes

You can also use the TagHelper without `IPagingModel` by specifying attributes directly:

```html
<paging
    page="@Model.CurrentPage"
    page-size="@Model.ItemsPerPage"
    total-items="@Model.TotalProducts"
    link-url="/Products" />
```

Or with ViewBag:

```html
<paging
    page="@ViewBag.Page"
    page-size="@ViewBag.PageSize"
    total-items="@ViewBag.Total" />
```

## JavaScript Modes

The Pager supports five JavaScript modes:

### HTMX (Default)

Uses HTMX for dynamic page updates without full page reloads:

```html
<paging model="Model"
        js-mode="HTMX"
        htmx-target="#content" />
```

### HTMX with Alpine.js

Combines HTMX with Alpine.js for enhanced interactivity:

```html
<paging model="Model"
        js-mode="HTMXWithAlpine"
        htmx-target="#content" />
```

### Alpine.js Only

Uses only Alpine.js (no HTMX):

```html
<paging model="Model"
        js-mode="Alpine" />
```

### Plain JavaScript

Uses vanilla JavaScript with no framework dependencies:

```html
<paging model="Model"
        js-mode="PlainJS" />
```

### No JavaScript

Uses standard HTML forms and links (zero JavaScript):

```html
<paging model="Model"
        js-mode="NoJS" />
```

## Localization

The library supports 8 languages out of the box:

```html
<!-- French -->
<paging model="Model" language="fr" />

<!-- German -->
<paging model="Model" language="de" />

<!-- Spanish -->
<paging model="Model" language="es" />

<!-- Italian -->
<paging model="Model" language="it" />

<!-- Portuguese -->
<paging model="Model" language="pt" />

<!-- Japanese -->
<paging model="Model" language="ja" />

<!-- Chinese Simplified -->
<paging model="Model" language="zh-Hans" />
```

Supported languages:
- **en** - English (default)
- **fr** - French
- **de** - German
- **es** - Spanish
- **it** - Italian
- **pt** - Portuguese
- **ja** - Japanese
- **zh-Hans** - Chinese (Simplified)

## Custom Summary Templates

You can customize the pagination summary text using placeholders:

```html
<paging model="Model"
        summary-template="Showing {startItem}-{endItem} of {totalItems} items" />
```

### Available Placeholders

- `{currentPage}` - Current page number
- `{totalPages}` - Total number of pages
- `{totalItems}` - Total number of items
- `{pageSize}` - Items per page
- `{startItem}` - First item number on current page
- `{endItem}` - Last item number on current page

### Examples

```html
<!-- Simple format -->
<paging model="Model"
        summary-template="Page {currentPage} / {totalPages}" />

<!-- Detailed format -->
<paging model="Model"
        summary-template="Items {startItem} to {endItem} ({totalItems} total)" />

<!-- Percentage format -->
<paging model="Model"
        summary-template="Viewing page {currentPage} of {totalPages} ({pageSize} per page)" />
```

## Styling Options

### TailwindCSS + DaisyUI (Default)

Uses full DaisyUI components (btn, join, badge, etc.):

```html
<paging model="Model" view-type="TailwindAndDaisy" />
```

### Pure TailwindCSS

Uses only TailwindCSS utility classes (no DaisyUI dependency):

```html
<paging model="Model" view-type="Tailwind" />
```

### Bootstrap

Uses Bootstrap styling:

```html
<paging model="Model" view-type="Bootstrap" />
```

### Plain CSS

Uses embedded plain CSS (no external dependencies):

```html
<paging model="Model" view-type="Plain" />
```

Include the CSS in your layout:

```html
@Html.PlainCSS()
```

### No JavaScript View

Zero-JavaScript view using forms and standard links:

```html
<paging model="Model" view-type="NoJS" />
```

### Custom View

Use your own custom view:

```html
<paging model="Model"
        view-type="Custom"
        use-local-view="true" />
```

Create `Views/Components/Pager/Default.cshtml` in your application.

## Advanced Features

### With Sorting

```html
<paging model="Model"
        order-by="@Model.OrderBy"
        descending="@Model.Descending" />
```

### With Search

```html
<form>
    <input type="text" name="search" value="@Model.SearchTerm" />
    <button type="submit">Search</button>
</form>

<paging model="Model" search-term="@Model.SearchTerm" />
```

### Custom Page Sizes

Control what page sizes appear in the dropdown:

```csharp
public class ProductsViewModel : IPagingModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public List<int> PageSizes { get; set; } = new() { 5, 10, 25, 50, 100 };
}
```

### Hide Page Size Selector

```html
<paging model="Model" show-pagesize="false" />
```

### Hide Summary

```html
<paging model="Model" show-summary="false" />
```

### Custom Button Text

```html
<paging model="Model"
        first-page-text="First"
        previous-page-text="Prev"
        next-page-text="Next"
        last-page-text="Last" />
```

### Disable First/Last Navigation

```html
<paging model="Model" first-last-navigation="false" />
```

### Disable Skip Navigation

```html
<paging model="Model" skip-forward-back-navigation="false" />
```

### More Pages to Display

```html
<paging model="Model" pages-to-display="10" />
```

## Examples

### Example 1: Basic Pagination

```html
@model ProductsViewModel

<paging model="Model" />
```

### Example 2: HTMX with Search

```html
@model ProductsViewModel

<div hx-target="#content">
    <form hx-get="/Products" hx-target="#content">
        <input type="text" name="search" value="@Model.SearchTerm" />
        <button type="submit">Search</button>
    </form>
</div>

<div id="content">
    @foreach (var product in Model.Products)
    {
        <div>@product.Name - @product.Price</div>
    }
</div>

<paging model="Model"
        htmx-target="#content"
        search-term="@Model.SearchTerm" />
```

### Example 3: Sorted Pagination

```html
@model ProductsViewModel

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
                           descending="@Model.Descending">
                Price
            </sortable-header>
        </tr>
    </thead>
    <tbody>
        @foreach (var product in Model.Products)
        {
            <tr>
                <td>@product.Name</td>
                <td>@product.Price</td>
            </tr>
        }
    </tbody>
</table>

<paging model="Model"
        order-by="@Model.OrderBy"
        descending="@Model.Descending" />
```

### Example 4: French Localization with Custom Template

```html
<paging model="Model"
        language="fr"
        summary-template="Affichage de {startItem} à {endItem} sur {totalItems} éléments" />
```

### Example 5: Bootstrap Style with Alpine.js

```html
<paging model="Model"
        view-type="Bootstrap"
        js-mode="Alpine" />
```

### Example 6: Minimal No-JS Pagination

```html
<paging model="Model"
        view-type="NoJS"
        js-mode="NoJS"
        show-pagesize="false"
        first-last-navigation="false"
        skip-forward-back-navigation="false" />
```

### Example 7: Large Datasets

```html
<paging model="Model"
        pages-to-display="10"
        max-page-size="500"
        summary-template="Showing {startItem}-{endItem} of {totalItems} records" />
```

## See Also

- [PageSize TagHelper](pagesize-taghelper.md) - Standalone page size selector
- [SortableHeader TagHelper](sortable-header-taghelper.md) - Sortable column headers
- [JavaScript Modes Guide](javascript-modes.md) - Detailed guide on JavaScript modes
- [Custom Views Guide](custom-views.md) - Creating your own views
