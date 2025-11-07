# Mostlylucid Paging TagHelper

[![NuGet](https://img.shields.io/nuget/v/mostlylucid.pagingtaghelper.svg)](https://www.nuget.org/packages/mostlylucid.pagingtaghelper/)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/github/license/scottgal/mostlylucid.pagingtaghelper)](LICENSE)

**The most flexible, powerful, and easy-to-use pagination library for ASP.NET Core.**

Built for modern web apps with HTMX, Alpine.js, or vanilla JavaScript support. Style it with TailwindCSS, DaisyUI, Bootstrap, or bring your own CSS. Zero configuration required to get started, infinitely customizable when you need it.

```html
<!-- That's it. Seriously. -->
<paging model="Model" />
```

[Live Demo](https://taghelpersample.mostlylucid.net/) | [Documentation](docs/) | [Blog Posts](https://www.mostlylucid.net/blog/category/PagingTagHelper)

---

## Features

### **Multiple UI Frameworks Out of the Box**
- **TailwindCSS + DaisyUI** - Modern, beautiful components with dark mode
- **Pure TailwindCSS** - Utility-first styling without component dependencies
- **Bootstrap** - Classic Bootstrap pagination styles
- **Plain CSS** - Custom injected styles for any project
- **Custom Views** - Complete control with your own Razor views

### **Progressive Enhancement with JavaScript Modes**
- **HTMX** - AJAX pagination without page reloads (default)
- **HTMX + Alpine.js** - Reactive state management with HTMX
- **Alpine.js** - Lightweight client-side navigation
- **Plain JavaScript** - Vanilla JS for maximum compatibility
- **No JavaScript** - Fully functional with forms and links only

### **Internationalization Built-In**
- 8 languages supported out of the box (EN, DE, ES, FR, IT, JA, PT, ZH-Hans)
- Custom localization with resource files or inline text
- `language="fr"` - Just set the culture
- Custom summary templates with placeholders

### **Developer Experience**
- **Zero Configuration** - Works immediately with sensible defaults
- **Type-Safe** - Full IntelliSense support in Razor views
- **Extensible** - Override anything, customize everything
- **Well-Tested** - 106 unit tests and counting
- **HTMX-First** - But works great without it

### **Bonus Features**
- **Sortable Headers** - Flip between ascending/descending with visual indicators
- **Page Size Selector** - Standalone component for changing items per page
- **Search Integration** - Preserve search terms across pages
- **Dark Mode** - Automatic dark mode support for all views

---

## Installation

```bash
dotnet add package mostlylucid.pagingtaghelper
```

**That's it!** No configuration files, no middleware, no setup. Just add the package and start using it.

---

## Quick Start

### 1. Create Your Model

Implement `IPagingModel` with your data:

```csharp
public class ProductListViewModel : IPagingModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public List<Product> Products { get; set; } = new();
}
```

### 2. Populate in Your Controller

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
        Page = page,
        PageSize = pageSize,
        TotalItems = totalProducts,
        Products = products
    };

    return View(model);
}
```

### 3. Add to Your View

```html
@model ProductListViewModel

<h1>Products</h1>

@foreach (var product in Model.Products)
{
    <div>@product.Name</div>
}

<paging model="Model" />
```

**Done!** You now have fully-functional pagination with HTMX support, page size selection, and localized text.

---

## Usage Examples

### HTMX with Partial Updates

```html
<div id="product-list">
    <partial name="_ProductList" model="Model" />
</div>

<paging model="Model"
        hx-target="#product-list"
        hx-swap="outerHTML" />
```

### Custom Language

```html
<paging model="Model" language="fr" />
```

### Custom Summary Template

```html
<paging model="Model"
        summary-template="Showing {startItem}-{endItem} of {totalItems} items" />
```

### Pure Tailwind (No DaisyUI)

```html
<paging model="Model" view-type="Tailwind" />
```

### No JavaScript (Progressive Enhancement)

```html
<paging model="Model" js-mode="NoJS" />
```

### Custom Text

```html
<paging model="Model"
        first-page-text="First"
        previous-page-text="Previous"
        next-page-text="Next"
        last-page-text="Last" />
```

### With Search

```html
<paging model="Model" search-term="@Model.SearchQuery" />
```

---

## Styling

The tag helper includes several built-in view types:

| View Type | Description | Dark Mode |
|-----------|-------------|-----------|
| `TailwindAndDaisy` | TailwindCSS + DaisyUI components (default) | Yes |
| `Tailwind` | Pure Tailwind utility classes | Yes |
| `Bootstrap` | Bootstrap 5 pagination | No |
| `Plain` | Custom injected CSS | Yes |
| `NoJS` | Form-based, zero JavaScript | Yes |

```html
<paging model="Model" view-type="Tailwind" />
```

### Custom Views

Create your own view in `Views/Shared/Components/Pager/`:

```html
<paging model="Model" view-type="Custom" use-local-view="true" />
```

[Read the Custom Views Guide](docs/custom-views.md)

---

## JavaScript Modes

Control how pagination behaves on the client:

| Mode | Description | Requirements |
|------|-------------|--------------|
| `HTMX` | AJAX requests without page reloads | htmx.js |
| `HTMXWithAlpine` | HTMX + reactive state | htmx.js + alpine.js |
| `Alpine` | Client-side navigation | alpine.js |
| `PlainJS` | Vanilla JavaScript | None |
| `NoJS` | Forms and links only | None |

```html
<paging model="Model" js-mode="Alpine" />
```

[Read the JavaScript Modes Guide](docs/javascript-modes.md)

---

## Localization

### Built-in Languages

Set the culture with one attribute:

```html
<paging model="Model" language="de" />  <!-- German -->
<paging model="Model" language="ja" />  <!-- Japanese -->
<paging model="Model" language="fr" />  <!-- French -->
```

**Supported:** English, German, Spanish, French, Italian, Japanese, Portuguese, Chinese (Simplified)

### Custom Text

Override any text inline:

```html
<paging model="Model"
        first-page-text="First"
        previous-page-text="Prev"
        next-page-text="Next"
        last-page-text="Last" />
```

### Custom Summary Templates

Use placeholders to build your own summary:

```html
<paging model="Model"
        summary-template="Page {currentPage} of {totalPages} - {totalItems} results" />
```

**Available Placeholders:**
- `{currentPage}` - Current page number
- `{totalPages}` - Total pages
- `{totalItems}` - Total item count
- `{pageSize}` - Items per page
- `{startItem}` - First item on page
- `{endItem}` - Last item on page

[Read the Localization Guide](docs/localization.md)

---

## Bonus: Sortable Headers

Add sortable column headers with visual flip indicators:

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
                           descending="@Model.Descending">
                Price
            </sortable-header>
        </tr>
    </thead>
</table>
```

Works seamlessly with HTMX or standard forms.

---

## Bonus: Page Size Selector

Use the page size component standalone:

```html
<page-size model="Model" />

<!-- With custom sizes -->
<page-size model="Model" page-size-steps="5,10,25,50,100" />

<!-- With HTMX target -->
<page-size model="Model" hx-target="#results" />
```

---

## Documentation

- [Getting Started](docs/getting-started.md) - Detailed setup and first steps
- [JavaScript Modes](docs/javascript-modes.md) - Deep dive into all JS modes
- [Custom Views](docs/custom-views.md) - Build your own pagination UI
- [Localization](docs/localization.md) - Multi-language support
- [Advanced Usage](docs/advanced-usage.md) - Power user features
- [API Reference](docs/api-reference.md) - Complete property reference

---

## Why Choose This Library?

### **For Beginners**
- Works immediately with zero configuration
- Sensible defaults for everything
- Clear, simple API

### **For Professionals**
- Full TypeScript/IntelliSense support
- 106 unit tests ensuring stability
- Backward compatible - won't break your code
- Production-ready and battle-tested

### **For Designers**
- Multiple CSS frameworks supported
- Dark mode built-in
- Fully customizable views
- Accessible by default

### **For Performance**
- Minimal JavaScript footprint
- Works without JavaScript entirely
- HTMX-optimized for instant navigation
- No heavy framework dependencies

---

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

---

## License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## Acknowledgments

Built by [Scott Galloway](https://www.mostlylucid.net)

- **HTMX** - For making AJAX simple again
- **Alpine.js** - For lightweight reactivity
- **TailwindCSS** - For utility-first styling
- **DaisyUI** - For beautiful components

---

## Support

- [Live Demo](https://taghelpersample.mostlylucid.net/)
- [Blog Posts](https://www.mostlylucid.net/blog/category/PagingTagHelper)
- [Issue Tracker](https://github.com/scottgal/mostlylucid.pagingtaghelper/issues)

**Made with ASP.NET Core Tag Helpers and View Components**
