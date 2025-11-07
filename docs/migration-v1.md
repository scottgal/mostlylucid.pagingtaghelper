# Migration Guide: Pre-v1 to v1.0

## Backward Compatibility Guarantee

**v1.0 is 100% backward compatible with pre-v1 code.** All existing tag helper usages will continue to work without any changes.

## Default Values (Pre-v1 Compatible)

All defaults match pre-v1 behavior:

### Pager TagHelper
```csharp
use-htmx="true"                      // HTMX enabled by default
view-type="TailwindAndDaisy"          // Default styling
show-pagesize="true"                  // Show page size selector
show-summary="true"                   // Show "Page 1 of 10" summary
first-last-navigation="true"          // Show First/Last buttons
skip-forward-back-navigation="true"   // Show ".." ellipsis
```

### ContinuationPager TagHelper
```csharp
use-htmx="true"                      // HTMX enabled by default
view-type="TailwindAndDaisy"          // Default styling
show-pagesize="true"                  // Show page size selector
show-page-number="true"               // Show numbered pages
show-summary="true"                   // Show page summary
enable-token-accumulation="true"      // Store token history
preserve-query-parameters="true"      // NEW in v1.0 - Preserve all query params
```

### PageSize TagHelper
```csharp
use-htmx="true"                      // HTMX enabled by default
view-type="TailwindAndDaisy"          // Default styling
```

## Deprecated Attributes (Still Supported)

### `use-htmx` Attribute

**Status:** Deprecated but fully functional

The `use-htmx` boolean attribute is deprecated in favor of the more flexible `js-mode` attribute.

**Pre-v1 Code (Still Works):**
```html
<!-- HTMX enabled -->
<pager model="Model" use-htmx="true" />

<!-- HTMX disabled, use plain JavaScript -->
<pager model="Model" use-htmx="false" />
```

**v1.0 Equivalent:**
```html
<!-- HTMX enabled -->
<pager model="Model" js-mode="HTMX" />

<!-- Plain JavaScript -->
<pager model="Model" js-mode="PlainJS" />
```

### Mapping: `use-htmx` to `js-mode`

The `use-htmx` attribute automatically maps to `js-mode`:

| Pre-v1 | v1.0 Equivalent |
|--------|----------------|
| `use-htmx="true"` (default) | `js-mode="HTMX"` |
| `use-htmx="false"` | `js-mode="PlainJS"` |

**If both are specified, `js-mode` takes precedence.**

## What's New in v1.0 (Optional Features)

### New JavaScript Modes

v1.0 adds more JavaScript framework options:

```html
<!-- HTMX only (same as use-htmx="true") -->
<pager model="Model" js-mode="HTMX" />

<!-- HTMX + Alpine.js combination -->
<pager model="Model" js-mode="HTMXWithAlpine" />

<!-- Alpine.js only (no HTMX) -->
<pager model="Model" js-mode="Alpine" />

<!-- Vanilla JavaScript (same as use-htmx="false") -->
<pager model="Model" js-mode="PlainJS" />

<!-- Zero JavaScript - forms and links only -->
<pager model="Model" js-mode="NoJS" />
```

### New View Types

```html
<!-- Pure Tailwind without DaisyUI -->
<pager model="Model" view-type="Tailwind" />

<!-- Zero JavaScript with form submission -->
<pager model="Model" view-type="NoJS" />
```

### Continuation Token Pagination

**NEW:** Support for NoSQL databases (Cosmos DB, DynamoDB, Azure Table Storage):

```html
<continuation-pager
    model="Model"
    hx-target="#results"
    show-page-number="true"
    max-history-pages="20"
    preserve-query-parameters="true" />
```

### Localization

**NEW:** Built-in localization for 8 languages:
- English, Spanish, French, German, Italian, Portuguese, Japanese, Chinese (Simplified)

Automatically uses current UI culture. No configuration needed!

### Custom Summary Templates

**NEW:** Customize the summary text with placeholders:

```html
<pager model="Model"
       summary-template="Showing {start}-{end} of {total} results (Page {page}/{totalPages})" />
```

Available placeholders: `{page}`, `{totalPages}`, `{total}`, `{start}`, `{end}`, `{pageSize}`

## Migration Steps (Optional - Only if you want new features)

### Step 1: Update NuGet Package

```bash
dotnet add package mostlylucid.pagingtaghelper --version 1.0.0
```

### Step 2: (Optional) Modernize Tag Helpers

Replace deprecated `use-htmx` with `js-mode`:

**Before:**
```html
<pager model="Model" use-htmx="true" />
<pager model="Model" use-htmx="false" />
```

**After:**
```html
<pager model="Model" js-mode="HTMX" />
<pager model="Model" js-mode="PlainJS" />
```

### Step 3: (Optional) Use New Features

Add continuation token pagination, localization, or new JavaScript modes as needed.

## Breaking Changes

**None!** v1.0 is fully backward compatible.

The only "breaking" change is that `ViewType.TailwindAndDaisy` now uses **full DaisyUI components** (`btn`, `join`, `badge`, etc.). If you want pure Tailwind without DaisyUI, use `ViewType.Tailwind` instead.

## Testing Your Migration

1. Update the NuGet package
2. Build your project (should succeed with no errors)
3. Run your application
4. Verify pagination works exactly as before
5. (Optional) Start using new v1.0 features

## Need Help?

- [Full Documentation](https://github.com/scottgal/mostlylucidimages)
- [Report Issues](https://github.com/scottgal/mostlylucidimages/issues)
- [Examples](https://github.com/scottgal/mostlylucidimages/tree/main/samples)
