# JavaScript Modes

The Paging TagHelper supports **5 different JavaScript modes**, allowing you to choose the perfect balance between functionality and dependencies for your project.

## Overview

| Mode | Framework | Page Reloads | Dependencies |
|------|-----------|--------------|--------------|
| `HTMX` | HTMX | No | htmx.js |
| `HTMXWithAlpine` | HTMX + Alpine.js | No | htmx.js + alpine.js |
| `Alpine` | Alpine.js | No | alpine.js |
| `PlainJS` | Vanilla JavaScript | No | None |
| `NoJS` | Forms/Links | Yes | None |

## Setting the Mode

Use the `js-mode` attribute:

```html
<paging model="Model" js-mode="HTMX" />
<paging model="Model" js-mode="Alpine" />
<paging model="Model" js-mode="NoJS" />
```

## HTMX Mode (Default)

**Best for:** Modern web apps with AJAX navigation

Uses HT MX to update page content without full page reloads.

### Setup

Include htmx.js in your layout:

```html
<script src="https://unpkg.com/htmx.org@2.0.4"></script>
```

### Usage

```html
<div id="results">
    <partial name="_ResultsList" model="Model" />
</div>

<paging model="Model"
        js-mode="HTMX"
        hx-target="#results"
        hx-swap="outerHTML" />
```

### Controller Support

Return partial views for HTMX requests:

```csharp
public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
{
    var model = await GetDataAsync(page, pageSize);

    if (Request.Headers["HX-Request"] == "true")
    {
        return PartialView("_ResultsList", model);
    }

    return View(model);
}
```

### Features
- No page reloads
- Browser history support (with `hx-push-url`)
- Loading indicators
- Error handling
- Smooth transitions

## HTMX with Alpine Mode

**Best for:** Apps needing both AJAX and reactive state

Combines HTMX for server requests with Alpine.js for client-side reactivity.

### Setup

```html
<script src="https://unpkg.com/htmx.org@2.0.4"></script>
<script src="https://unpkg.com/alpinejs@3.13.0/dist/cdn.min.js" defer></script>
```

### Usage

```html
<div x-data="{ loading: false }">
    <div id="results"
         hx-indicator=".spinner"
         @htmx:before-request="loading = true"
         @htmx:after-request="loading = false">
        <partial name="_ResultsList" model="Model" />
    </div>

    <div x-show="loading" class="spinner">Loading...</div>

    <paging model="Model"
            js-mode="HTMXWithAlpine"
            hx-target="#results" />
</div>
```

### Features
- All HTMX features
- Plus reactive state management
- Custom event handling
- Conditional rendering
- Animations

## Alpine Mode

**Best for:** Client-side only navigation

Uses Alpine.js to navigate without server requests until submission.

### Setup

```html
<script src="https://unpkg.com/alpinejs@3.13.0/dist/cdn.min.js" defer></script>
```

### Usage

```html
<paging model="Model" js-mode="Alpine" />
```

### How It Works

Alpine mode uses `window.location.href` to navigate, causing a standard page load but with Alpine's event system for smooth UX.

### Features
- Lightweight (no HTMX dependency)
- Reactive bindings
- Simple setup
- Good for smaller apps

## Plain JavaScript Mode

**Best for:** Maximum compatibility, no framework dependencies

Uses vanilla JavaScript with zero external dependencies.

### Setup

Add the JavaScript snippet helper:

```html
@Html.PageSizeOnchangeSnippet()
```

### Usage

```html
<paging model="Model" js-mode="PlainJS" />

@Html.PageSizeOnchangeSnippet()
```

### How It Works

The snippet adds a simple event listener to handle page size changes:

```javascript
document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('.page-size-container select').forEach(select => {
        select.addEventListener('change', function() {
            const url = this.closest('.page-size-container')
                           .querySelector('.linkUrl').value;
            const pageSize = this.value;
            window.location.href = `${url}?pageSize=${pageSize}`;
        });
    });
});
```

### Features
- Zero dependencies
- Works everywhere
- Simple and predictable
- Easy to customize

## No JavaScript Mode

**Best for:** Progressive enhancement, accessibility, JavaScript-disabled environments

Works with pure HTML forms and links. No JavaScript whatsoever.

### Usage

```html
<paging model="Model" js-mode="NoJS" />
```

### How It Works

- **Pagination links** - Standard `<a>` tags
- **Page size selector** - HTML form with submit button

### Features
- Works with JavaScript disabled
- Perfect for accessibility
- SEO-friendly
- Progressive enhancement
- Maximum compatibility

### Example Output

```html
<!-- Page navigation uses standard links -->
<a href="/products?page=2&pageSize=10">2</a>

<!-- Page size uses a form -->
<form method="get" action="/products">
    <select name="pageSize">
        <option value="10">10</option>
        <option value="25">25</option>
    </select>
    <button type="submit">Change</button>
</form>
```

## Backward Compatibility

The deprecated `use-htmx` attribute is still supported:

```html
<!-- Old way (still works) -->
<paging model="Model" use-htmx="true" />
<paging model="Model" use-htmx="false" />

<!-- New way (recommended) -->
<paging model="Model" js-mode="HTMX" />
<paging model="Model" js-mode="PlainJS" />
```

## Choosing the Right Mode

### Use **HTMX** when:
- You want SPA-like experience
- You're okay with one dependency (htmx.js)
- You want server-rendered content
- You need browser history support

### Use **HTMXWithAlpine** when:
- You need HTMX + reactive state
- You have complex client-side interactions
- You want the best of both worlds

### Use **Alpine** when:
- You already use Alpine.js
- You want lightweight reactivity
- HTMX is overkill for your needs

### Use **PlainJS** when:
- You want zero dependencies
- You need maximum control
- You're building a simple site

### Use **NoJS** when:
- You require accessibility
- You support JavaScript-disabled users
- You want progressive enhancement
- SEO is critical

## Advanced: Per-Component Modes

You can use different modes for different pagers on the same page:

```html
<!-- Main listing with HTMX -->
<paging model="Model.Products"
        js-mode="HTMX"
        id="products-pager"
        hx-target="#product-list" />

<!-- Sidebar with NoJS (for accessibility) -->
<paging model="Model.RecentItems"
        js-mode="NoJS"
        id="recent-pager" />
```

## Troubleshooting

### HTMX Mode Not Working

1. **Check htmx.js is loaded:**
```html
<script src="https://unpkg.com/htmx.org@2.0.4"></script>
```

2. **Verify hx-target exists:**
```html
<div id="my-target"><!-- Must exist --></div>
<paging hx-target="#my-target" />
```

3. **Controller returns partial for HTMX:**
```csharp
if (Request.Headers["HX-Request"] == "true")
{
    return PartialView("_Partial", model);
}
```

### PlainJS Not Working

Ensure the snippet is included:
```html
@Html.PageSizeOnchangeSnippet()
```

### Alpine Not Working

1. **Check Alpine is loaded:**
```html
<script src="https://unpkg.com/alpinejs@3.13.0/dist/cdn.min.js" defer></script>
```

2. **Use defer attribute** - Alpine needs DOM to be ready

## Performance Comparison

| Mode | Initial Load | Navigation | Bundle Size |
|------|--------------|------------|-------------|
| HTMX | Fast | Fastest | ~14KB |
| HTMXWithAlpine | Fast | Fastest | ~54KB |
| Alpine | Fast | Fast | ~40KB |
| PlainJS | Fast | Fast | ~1KB |
| NoJS | Fast | Slow (full reload) | 0KB |

##Summary

All modes are production-ready and battle-tested. Choose based on your project's needs:

- **Most users:** `HTMX` (default, best UX)
- **Need reactivity:** `HTMXWithAlpine`
- **Already using Alpine:** `Alpine`
- **No dependencies:** `PlainJS`
- **Maximum accessibility:** `NoJS`

All modes maintain backward compatibility and can be switched without changing your controller code.
