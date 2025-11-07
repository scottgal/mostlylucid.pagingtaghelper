# Getting Started

This guide will walk you through setting up and using the Mostlylucid Paging TagHelper in your ASP.NET Core application.

> **Upgrading from pre-v1?** Great news! v1.0 is 100% backward compatible. Just update your NuGet package and you're done. All existing code will continue to work without any changes. [See migration guide](migration-v1.md)

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package mostlylucid.pagingtaghelper
```

Or via Package Manager Console:

```
Install-Package mostlylucid.pagingtaghelper
```

**That's it!** The tag helper is automatically registered and ready to use.

## Two Ways to Use the Paging TagHelper

There are two approaches: using the **full model** (recommended) or using **individual attributes** (manual approach).

---

## Approach 1: Using IPagingModel (Recommended)

This is the cleanest approach - your ViewModel implements `IPagingModel` and you pass it directly.

### 1. Create Your Model

Your view model implements `IPagingModel`:

```csharp
using mostlylucid.pagingtaghelper.Models;

public class ProductListViewModel : IPagingModel
{
    // Required IPagingModel properties
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }

    // Your data
    public List<Product> Products { get; set; } = new();
}
```

### 2. Controller Action

Populate your model with data and pagination info:

```csharp
public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
{
    // Get total count
    var totalProducts = await _db.Products.CountAsync();

    // Get page of data
    var products = await _db.Products
        .OrderBy(p => p.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    // Build view model
    var model = new ProductListViewModel
    {
        Page = page,
        PageSize = pageSize,
        TotalItems = totalProducts,
        Products = products
    };

    return View(model);
}
```

### 3. Razor View

Add the tag helper to your view:

```html
@model ProductListViewModel

<h1>Products (@Model.TotalItems total)</h1>

<div class="product-grid">
    @foreach (var product in Model.Products)
    {
        <div class="product-card">
            <h3>@product.Name</h3>
            <p>@product.Price.ToString("C")</p>
        </div>
    }
</div>

<!-- That's it! -->
<paging model="Model" />
```

---

## Approach 2: Using Individual Attributes (Manual)

If you don't want to implement `IPagingModel`, you can pass pagination data via attributes.

### 1. Create Your Model

Your ViewModel doesn't need to implement any interface:

```csharp
public class ProductListViewModel
{
    // No interface required!
    public List<Product> Products { get; set; } = new();

    // Keep these for the view to pass to the tag helper
    public int CurrentPage { get; set; }
    public int ItemsPerPage { get; set; }
    public int Total { get; set; }
}
```

### 2. Controller Action

Same as before, but with your own property names:

```csharp
public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
{
    var totalProducts = await _db.Products.CountAsync();
    var products = await _db.Products
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var model = new ProductListViewModel
    {
        CurrentPage = page,
        ItemsPerPage = pageSize,
        Total = totalProducts,
        Products = products
    };

    return View(model);
}
```

### 3. Razor View

Pass each value individually:

```html
@model ProductListViewModel

<h1>Products (@Model.Total total)</h1>

<div class="product-grid">
    @foreach (var product in Model.Products)
    {
        <div class="product-card">
            <h3>@product.Name</h3>
            <p>@product.Price.ToString("C")</p>
        </div>
    }
</div>

<!-- Pass individual attributes -->
<paging page="@Model.CurrentPage"
        page-size="@Model.ItemsPerPage"
        total-items="@Model.Total" />
```

### Manual Approach with No ViewModel Properties

You can even skip storing pagination in your ViewModel entirely:

```csharp
// Controller
public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
{
    var model = new ProductListViewModel
    {
        Products = await GetProductsAsync(page, pageSize)
    };

    ViewBag.Page = page;
    ViewBag.PageSize = pageSize;
    ViewBag.TotalProducts = await _db.Products.CountAsync();

    return View(model);
}
```

```html
<!-- View -->
<paging page="@ViewBag.Page"
        page-size="@ViewBag.PageSize"
        total-items="@ViewBag.TotalProducts" />
```

---

## Comparison

| Aspect | IPagingModel (Approach 1) | Individual Attributes (Approach 2) |
|--------|---------------------------|-------------------------------------|
| **Code** | Cleaner | More verbose |
| **Type Safety** | Better (IntelliSense) | Good |
| **Flexibility** | Less (fixed interface) | More (name your props) |
| **Boilerplate** | Less | More |
| **Best For** | New projects | Existing ViewModels |

### When to Use Each Approach

**Use IPagingModel when:**
- Starting a new project
- You want clean, consistent code
- You don't mind implementing an interface
- You want IntelliSense to guide you

**Use Individual Attributes when:**
- Working with existing ViewModels
- You can't change the ViewModel
- You want maximum flexibility
- You're integrating with legacy code

---

## What You Get

With just `<paging model="Model" />`, you automatically get:

✅ **Page Navigation** - First, Previous, numbered pages, Next, Last
✅ **Page Size Selector** - Dropdown to change items per page
✅ **Summary Text** - "Page 1 of 10 (Total items: 256)"
✅ **HTMX Support** - AJAX navigation without page reloads
✅ **Localization** - Based on current UI culture
✅ **Dark Mode** - Automatic support for dark themes
✅ **Accessibility** - Proper ARIA labels and semantic HTML

## Next Steps

Now that you have basic pagination working, explore more features:

- **[JavaScript Modes](javascript-modes.md)** - Choose how pagination behaves (HTMX, Alpine, PlainJS, NoJS)
- **[Custom Views](custom-views.md)** - Style pagination to match your design
- **[Localization](localization.md)** - Multi-language support and custom text
- **[Advanced Usage](advanced-usage.md)** - Search, sorting, and more

## Common Patterns

### HTMX with Partial Views

For SPA-like experience without page reloads:

**Controller:**
```csharp
public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
{
    var model = await GetProductsAsync(page, pageSize);

    // Return partial for HTMX requests
    if (Request.Headers["HX-Request"] == "true")
    {
        return PartialView("_ProductList", model);
    }

    return View(model);
}
```

**View:**
```html
<div id="product-list">
    <partial name="_ProductList" model="Model" />
</div>

<paging model="Model" hx-target="#product-list" />
```

**_ProductList.cshtml:**
```html
@model ProductListViewModel

@foreach (var product in Model.Products)
{
    <div>@product.Name</div>
}

<paging model="Model" hx-target="#product-list" />
```

### With Search

Preserve search queries across pagination:

```csharp
public class ProductListViewModel : IPagingModel
{
    public string? SearchQuery { get; set; }
    // ... other properties
}
```

```html
<paging model="Model" search-term="@Model.SearchQuery" />
```

### Multiple Pagers on One Page

Give each pager a unique ID:

```html
<paging model="Model.TopProducts" id="top-products-pager" />

<!-- Other content -->

<paging model="Model.NewProducts" id="new-products-pager" />
```

## Troubleshooting

### Pager Not Showing

1. **Check TotalItems** - Must be > 0
2. **Check Page** - Must be >= 1
3. **Check PageSize** - Must be > 0

### Styling Issues with TailwindCSS

**Problem:** TailwindCSS's tree-shaking (PurgeCSS) removes classes that aren't found in your source files. Since the pager views are embedded in the library DLL, Tailwind can't detect which classes are needed.

**Solution:** Include this placeholder span in your `_Layout.cshtml` to preserve all required classes:

```html
<!--
    Preserve Tailwind & DaisyUI classes used in embedded pager views.
    Without this, TailwindCSS tree-shaking will remove classes that are only
    referenced in the embedded Razor views (which are compiled into the library DLL).
-->
<span class="hidden
    btn btn-sm btn-active btn-disabled btn-primary btn-outline join join-item badge badge-ghost badge-sm select select-primary select-sm
    inline-flex flex items-center justify-center gap-2 px-4 py-2 mr-2 text-sm font-medium shadow-sm rounded-md rounded-l-lg rounded-r-lg rounded-full border border-gray-300
    text-white text-gray-700 text-gray-400 text-gray-600 bg-white bg-gray-100 bg-blue-600 border-blue-600
    hover:bg-gray-50 hover:bg-blue-700 focus:z-10 focus:ring-2 focus:ring-blue-500 focus:outline-none focus:border-blue-500 cursor-not-allowed whitespace-nowrap block w-auto
    dark:bg-gray-800 dark:bg-gray-700 dark:bg-blue-500 dark:text-gray-300 dark:text-gray-500 dark:text-gray-200 dark:text-white
    dark:border-gray-600 dark:border-blue-500 dark:hover:bg-gray-700 dark:hover:bg-blue-600 dark:focus:ring-blue-400
    dark:btn-accent dark:btn-outline dark:btn-disabled dark:btn-primary dark:btn-active">
</span>
```

**What's Preserved:**
- **DaisyUI components** (for `view-type="TailwindAndDaisy"`): `btn`, `join`, `badge`, `select` and variants
- **Pure Tailwind utilities** (for `view-type="Tailwind"`): Layout, spacing, colors, borders, focus states, dark mode
- **Not needed for** `Bootstrap`, `Plain`, or `NoJS` view types

### HTMX Not Working

1. **Include htmx.js** in your layout:
```html
<script src="https://unpkg.com/htmx.org@2.0.4"></script>
```

2. **Check hx-target** - Must point to valid element ID

3. **Controller must support HTMX** - Return partial views for HTMX requests

## Quick Reference

### Minimal Example
```html
<paging model="Model" />
```

### With Custom Page Size
```html
<paging model="Model" page-size="25" />
```

### Different View Type
```html
<paging model="Model" view-type="Tailwind" />
```

### Custom Text
```html
<paging model="Model"
        first-page-text="Start"
        last-page-text="End" />
```

### Without Page Size Selector
```html
<paging model="Model" show-pagesize="false" />
```

### Without Summary
```html
<paging model="Model" show-summary="false" />
```

---

Ready to dive deeper? Check out the [JavaScript Modes guide](javascript-modes.md) next!
