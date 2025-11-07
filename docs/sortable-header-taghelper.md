# SortableHeader TagHelper

The `<sortable-header>` TagHelper creates sortable column headers for tables with automatic sort direction indicators and URL management.

## Table of Contents

- [Basic Usage](#basic-usage)
- [Attributes Reference](#attributes-reference)
- [Sort Direction Indicators](#sort-direction-indicators)
- [JavaScript Modes](#javascript-modes)
- [Integration with Pagination](#integration-with-pagination)
- [Styling and Customization](#styling-and-customization)
- [Examples](#examples)

## Basic Usage

The simplest usage for a table column:

```html
<table>
    <thead>
        <tr>
            <sortable-header column="Name">Product Name</sortable-header>
            <sortable-header column="Price">Price</sortable-header>
            <sortable-header column="CreatedDate">Date</sortable-header>
        </tr>
    </thead>
    <tbody>
        @foreach (var product in Model.Products)
        {
            <tr>
                <td>@product.Name</td>
                <td>@product.Price</td>
                <td>@product.CreatedDate</td>
            </tr>
        }
    </tbody>
</table>
```

## Attributes Reference

| Attribute | Type | Default | Description |
|-----------|------|---------|-------------|
| `column` | `string` | required | The property/column name to sort by |
| `current-order-by` | `string?` | null | The currently active sort column |
| `descending` | `bool` | false | Whether current sort is descending |
| `use-htmx` | `bool` | true | Enable HTMX for AJAX sorting |
| `action` | `string?` | null | ASP.NET Core MVC action name |
| `controller` | `string?` | null | ASP.NET Core MVC controller name |
| `hx-action` | `string?` | null | HTMX-specific action (generates hx-get="#") |
| `hx-controller` | `string?` | null | HTMX-specific controller |
| `auto-append-querystring` | `bool` | true | Automatically preserve query parameters |
| `chevron-up-class` | `string?` | "bx bx-sm bx-chevron-up" | CSS class for ascending indicator |
| `chevron-down-class` | `string?` | "bx bx-sm bx-chevron-down" | CSS class for descending indicator |
| `chevron-unsorted-class` | `string?` | "bx bx-sm bx-sort-alt-2" | CSS class for unsorted indicator |
| `tag-class` | `string?` | "cursor-pointer flex items-center gap-2" | CSS class for the header element |

## Sort Direction Indicators

The TagHelper automatically displays sort direction indicators:

- **Unsorted**: Shows an unsorted icon (default: ⇅)
- **Ascending**: Shows an up arrow (↑) when sorted A-Z or 0-9
- **Descending**: Shows a down arrow (↓) when sorted Z-A or 9-0

### Sort Logic

1. **First click**: Sorts ascending
2. **Second click**: Sorts descending
3. **Third click** (optional): Returns to default/unsorted state

The TagHelper automatically flips between ascending and descending:

```html
<sortable-header
    column="Name"
    current-order-by="@Model.OrderBy"
    descending="@Model.Descending">
    Product Name
</sortable-header>
```

When `current-order-by="Name"` and `descending="false"`, clicking generates:
- URL parameter: `?orderBy=Name&descending=true`

## JavaScript Modes

### HTMX Mode (Default)

Sorts without full page reload:

```html
<sortable-header
    column="Name"
    current-order-by="@Model.OrderBy"
    descending="@Model.Descending"
    use-htmx="true">
    Product Name
</sortable-header>
```

Generates:
```html
<a href="/Products?orderBy=Name&descending=false"
   hx-vals='{"orderBy":"Name","descending":false}'
   class="cursor-pointer flex items-center gap-2">
    Product Name <i class="bx bx-sm bx-sort-alt-2"></i>
</a>
```

### Non-HTMX Mode

Uses standard links with full page reload:

```html
<sortable-header
    column="Name"
    use-htmx="false">
    Product Name
</sortable-header>
```

## Integration with Pagination

The SortableHeader TagHelper works seamlessly with pagination by preserving sort state:

### Controller

```csharp
public class ProductsController : Controller
{
    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = 10,
        string? orderBy = "Name",
        bool descending = false,
        string? search = null)
    {
        var query = _context.Products.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        // Apply sorting
        query = orderBy switch
        {
            "Name" => descending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            "Price" => descending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            "CreatedDate" => descending
                ? query.OrderByDescending(p => p.CreatedDate)
                : query.OrderBy(p => p.CreatedDate),
            _ => query.OrderBy(p => p.Name)
        };

        var viewModel = new ProductsViewModel
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = await query.CountAsync(),
            OrderBy = orderBy,
            Descending = descending,
            SearchTerm = search,
            Products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
        };

        return View(viewModel);
    }
}
```

### View

```html
@model ProductsViewModel

<table>
    <thead>
        <tr>
            <sortable-header
                column="Name"
                current-order-by="@Model.OrderBy"
                descending="@Model.Descending">
                Product Name
            </sortable-header>
            <sortable-header
                column="Price"
                current-order-by="@Model.OrderBy"
                descending="@Model.Descending">
                Price
            </sortable-header>
            <sortable-header
                column="CreatedDate"
                current-order-by="@Model.OrderBy"
                descending="@Model.Descending">
                Created Date
            </sortable-header>
        </tr>
    </thead>
    <tbody>
        @foreach (var product in Model.Products)
        {
            <tr>
                <td>@product.Name</td>
                <td>@product.Price.ToString("C")</td>
                <td>@product.CreatedDate.ToString("d")</td>
            </tr>
        }
    </tbody>
</table>

<paging model="Model"
        order-by="@Model.OrderBy"
        descending="@Model.Descending" />
```

### ViewModel

```csharp
public class ProductsViewModel : IPagedOrderableModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public string? OrderBy { get; set; }
    public bool Descending { get; set; }
    public string? SearchTerm { get; set; }
    public ViewType ViewType { get; set; } = ViewType.TailwindAndDaisy;

    public List<Product> Products { get; set; } = new();

    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}
```

## Styling and Customization

### Custom Icons with Boxicons

The default uses Boxicons:

```html
<sortable-header
    column="Name"
    chevron-up-class="bx bx-chevron-up"
    chevron-down-class="bx bx-chevron-down"
    chevron-unsorted-class="bx bx-sort-alt-2">
    Product Name
</sortable-header>
```

### Custom Icons with Font Awesome

```html
<sortable-header
    column="Name"
    chevron-up-class="fa fa-sort-up"
    chevron-down-class="fa fa-sort-down"
    chevron-unsorted-class="fa fa-sort">
    Product Name
</sortable-header>
```

### Custom Icons with Bootstrap Icons

```html
<sortable-header
    column="Name"
    chevron-up-class="bi bi-arrow-up"
    chevron-down-class="bi bi-arrow-down"
    chevron-unsorted-class="bi bi-arrow-down-up">
    Product Name
</sortable-header>
```

### Custom CSS Classes

```html
<sortable-header
    column="Name"
    tag-class="sortable-column hover:bg-gray-100">
    Product Name
</sortable-header>
```

### Without Icons

Set all icon classes to empty string:

```html
<sortable-header
    column="Name"
    chevron-up-class=""
    chevron-down-class=""
    chevron-unsorted-class="">
    Product Name
</sortable-header>
```

## Query String Preservation

The TagHelper automatically preserves existing query parameters:

```html
<!-- Current URL: /Products?search=laptop&category=electronics&page=2 -->

<sortable-header
    column="Price"
    auto-append-querystring="true">
    Price
</sortable-header>

<!-- Generates: /Products?orderBy=Price&descending=false&search=laptop&category=electronics&page=2 -->
```

Disable preservation if needed:

```html
<sortable-header
    column="Price"
    auto-append-querystring="false">
    Price
</sortable-header>

<!-- Generates: /Products?orderBy=Price&descending=false -->
```

## Examples

### Example 1: Basic Sortable Table

```html
<table class="table">
    <thead>
        <tr>
            <sortable-header column="Id">#</sortable-header>
            <sortable-header column="Name">Name</sortable-header>
            <sortable-header column="Email">Email</sortable-header>
        </tr>
    </thead>
    <tbody>
        @foreach (var user in Model.Users)
        {
            <tr>
                <td>@user.Id</td>
                <td>@user.Name</td>
                <td>@user.Email</td>
            </tr>
        }
    </tbody>
</table>
```

### Example 2: With Current Sort State

```html
<table>
    <thead>
        <tr>
            <sortable-header
                column="Name"
                current-order-by="@Model.OrderBy"
                descending="@Model.Descending">
                Product Name
            </sortable-header>
            <sortable-header
                column="Price"
                current-order-by="@Model.OrderBy"
                descending="@Model.Descending">
                Price
            </sortable-header>
        </tr>
    </thead>
</table>
```

### Example 3: HTMX with Target

```html
<div id="products-table">
    <table>
        <thead>
            <tr>
                <sortable-header
                    column="Name"
                    current-order-by="@Model.OrderBy"
                    descending="@Model.Descending"
                    hx-action="Index"
                    hx-controller="Products">
                    Product Name
                </sortable-header>
            </tr>
        </thead>
        <tbody>
            @* Products *@
        </tbody>
    </table>
</div>
```

### Example 4: Custom Styling with TailwindCSS

```html
<table class="w-full">
    <thead class="bg-gray-100 dark:bg-gray-800">
        <tr>
            <sortable-header
                column="Name"
                current-order-by="@Model.OrderBy"
                descending="@Model.Descending"
                tag-class="px-4 py-2 hover:bg-gray-200 dark:hover:bg-gray-700 cursor-pointer flex items-center gap-2"
                chevron-up-class="text-blue-500"
                chevron-down-class="text-blue-500"
                chevron-unsorted-class="text-gray-400">
                Name
            </sortable-header>
        </tr>
    </thead>
</table>
```

### Example 5: Multi-Column Sorting Priority

```html
<table>
    <thead>
        <tr>
            <sortable-header
                column="Category"
                current-order-by="@Model.PrimarySort"
                descending="@Model.PrimaryDescending">
                Category
            </sortable-header>
            <sortable-header
                column="Name"
                current-order-by="@Model.SecondarySort"
                descending="@Model.SecondaryDescending">
                Name
            </sortable-header>
        </tr>
    </thead>
</table>
```

### Example 6: With Search and Filters

```html
<form method="get">
    <input type="text" name="search" value="@Model.SearchTerm" placeholder="Search..." />
    <select name="category">
        <option value="">All Categories</option>
        <option value="Electronics">Electronics</option>
        <option value="Books">Books</option>
    </select>
    <button type="submit">Filter</button>
</form>

<table>
    <thead>
        <tr>
            <sortable-header
                column="Name"
                current-order-by="@Model.OrderBy"
                descending="@Model.Descending">
                Product
            </sortable-header>
            <sortable-header
                column="Price"
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
                <td>@product.Price.ToString("C")</td>
            </tr>
        }
    </tbody>
</table>

<paging model="Model"
        search-term="@Model.SearchTerm"
        order-by="@Model.OrderBy"
        descending="@Model.Descending" />
```

### Example 7: Accessibility Enhanced

```html
<table>
    <thead>
        <tr>
            <th scope="col">
                <sortable-header
                    column="Name"
                    current-order-by="@Model.OrderBy"
                    descending="@Model.Descending"
                    tag-class="cursor-pointer flex items-center gap-2"
                    aria-label="Sort by product name">
                    Product Name
                </sortable-header>
            </th>
            <th scope="col">
                <sortable-header
                    column="Price"
                    current-order-by="@Model.OrderBy"
                    descending="@Model.Descending"
                    aria-label="Sort by price">
                    Price
                </sortable-header>
            </th>
        </tr>
    </thead>
</table>
```

### Example 8: Mobile-Friendly

```html
<div class="overflow-x-auto">
    <table class="min-w-full">
        <thead>
            <tr>
                <sortable-header
                    column="Name"
                    current-order-by="@Model.OrderBy"
                    descending="@Model.Descending"
                    tag-class="px-2 py-1 text-sm cursor-pointer flex items-center gap-1">
                    <span class="hidden sm:inline">Product Name</span>
                    <span class="sm:hidden">Name</span>
                </sortable-header>
            </tr>
        </thead>
    </table>
</div>
```

## Advanced Patterns

### Dynamic Column Generation

```csharp
@{
    var columns = new Dictionary<string, string>
    {
        { "Name", "Product Name" },
        { "Price", "Price" },
        { "Stock", "In Stock" },
        { "CreatedDate", "Date Added" }
    };
}

<table>
    <thead>
        <tr>
            @foreach (var col in columns)
            {
                <sortable-header
                    column="@col.Key"
                    current-order-by="@Model.OrderBy"
                    descending="@Model.Descending">
                    @col.Value
                </sortable-header>
            }
        </tr>
    </thead>
</table>
```

### Conditional Sorting

Some columns might not be sortable:

```html
<table>
    <thead>
        <tr>
            <sortable-header
                column="Name"
                current-order-by="@Model.OrderBy"
                descending="@Model.Descending">
                Name
            </sortable-header>
            <th>Actions</th> <!-- Not sortable -->
        </tr>
    </thead>
</table>
```

## See Also

- [Pager TagHelper](pager-taghelper.md) - Full pagination controls
- [PageSize TagHelper](pagesize-taghelper.md) - Standalone page size selector
- [JavaScript Modes Guide](javascript-modes.md) - Detailed guide on JavaScript modes
- [Custom Views Guide](custom-views.md) - Creating your own views
