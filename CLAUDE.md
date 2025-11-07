# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an ASP.NET Core TagHelper library for pagination controls, published as a NuGet package. It provides three main components:
- **PagerTagHelper**: Full pagination controls with page navigation
- **PageSizeTagHelper**: Standalone page size selector
- **SortableHeaderTagHelper**: Column headers with sorting capability

The library is designed to work seamlessly with HTMX but can also function without it.

## Build and Development Commands

### Building the Project
```bash
# Build the entire solution
dotnet build mostlylucid.pagingtaghelper.sln

# Build specific projects
dotnet build mostlylucid.pagingtaghelper/mostlylucid.pagingtaghelper.csproj
dotnet build mostlylucid.pagingtaghelper.sample/mostlylucid.pagingtaghelper.sample.csproj
```

### Testing
```bash
# Run all tests
dotnet test mostlylucid.pagingtaghelper.tests/mostlylucid.pagingtaghelper.tests.csproj
```

### Packaging
```bash
# Create NuGet package (automatically generated on Release builds)
dotnet pack mostlylucid.pagingtaghelper/mostlylucid.pagingtaghelper.csproj -c Release
```

### Sample Application
The sample app demonstrates all features and uses TailwindCSS with DaisyUI:

```bash
# Navigate to sample directory
cd mostlylucid.pagingtaghelper.sample

# Install npm dependencies
npm install

# Build CSS and JS for development
npm run dev

# Watch mode (auto-rebuild on changes)
npm run watch

# Production build
npm run build

# Run the sample application
dotnet run
```

Note: The sample project automatically runs `npm run dev` or `npm run build` via MSBuild targets before compilation.

## Architecture and Code Structure

### Multi-Target Framework Support
The main library targets both `net8.0` and `net9.0` to support different ASP.NET Core versions.

### TagHelper Architecture
The library uses a ViewComponent pattern where TagHelpers delegate rendering to ViewComponents:

1. **TagHelper Layer** (`Components/*TagHelper.cs`): Parse attributes, build view models, invoke ViewComponents
2. **ViewComponent Layer** (`Components/*ViewComponent.cs`): Select appropriate view and render
3. **View Layer** (`Areas/Components/Views/*`): Razor views for different CSS frameworks

### View Type System
Views can be switched via the `ViewType` enum:
- `TailwindAndDaisy` (default): TailwindCSS + DaisyUI styling with full DaisyUI components (join, badge, etc.)
- `Tailwind`: Pure TailwindCSS styling without DaisyUI dependencies
- `Bootstrap`: Bootstrap styling
- `Plain`: Basic CSS (embedded in the library)
- `NoJS`: Zero-JavaScript view using forms and plain CSS (no HTMX, no JavaScript required)
- `Custom`: Use a local view from the consuming application

**Key differences between TailwindAndDaisy and Tailwind:**
- `TailwindAndDaisy`: Uses DaisyUI components like `btn`, `join`, `badge`, `select`, etc. Requires DaisyUI plugin.
- `Tailwind`: Uses only standard Tailwind utility classes. No DaisyUI dependency required.

### Model Hierarchy
```
IPagingModel (interface)
├── Page, TotalItems, PageSize, ViewType, LinkUrl
└── IPagingModel<T> (generic data container)

BaseTagModel (abstract)
├── Core paging properties
├── PageSizeModel
│   └── UseHtmx, LinkUrl
└── PagerViewModel (extends PageSizeModel)
    └── Navigation display options, sorting, summary
```

### Key Components

**PagerTagHelper** (`Components/PagerTagHelper.cs`):
- Accepts either an `IPagingModel` or individual properties
- Supports HTMX attributes (hx-target, hx-boost, etc.)
- Customizable navigation text and display options
- Invokes `PagerViewComponent` for rendering

**PagerViewComponent** (`Components/PagerViewComponent.cs`):
- View selection logic based on `ViewType` and `UseLocalView`
- Validates custom views exist before rendering

**PageSizeTagHelper** (`Components/PageSizeTagHelper.cs`):
- Standalone component for page size selection
- Can be used independently or as part of PagerTagHelper

**SortableHeaderTagHelper** (`Components/SortableHeaderTagHelper.cs`):
- Adds sortable column headers with direction indicators
- Integrates with HTMX for dynamic sorting

### Extensions

**QueryableExtensions** (`Extensions/QueryableExtensions.cs`):
- Helper methods for paginating IQueryable collections

**HtmlExtensions** (`Extensions/HtmlExtensions.cs`):
- `@Html.PageSizeOnchangeSnippet()`: JavaScript for non-HTMX page size handling
- `@Html.PlainCSS()`: Injects embedded CSS for PlainView

### Embedded Resources
The library embeds CSS and JavaScript files that can be injected into consuming apps:
- `CSS/PlainView.css` and `CSS/PlainViewMin.css`: Styling for Plain view (also used by NoJS view)
- `JavaScript/PagsizeOnchange.js`: Form submission handling without HTMX
- `JavaScript/HTMXPageSizeChange.js`: HTMX-specific handling

### NoJS View Type
The `NoJS` view type provides a completely JavaScript-free pagination experience:

**Features:**
- Uses standard HTML anchor links for page navigation
- Wraps page size selector in a `<form>` with a submit button
- Automatically resets to page 1 when changing page size
- Preserves existing query parameters (search, orderBy, etc.)
- Uses PlainView.css for styling

**When to Use:**
- Accessibility requirements demanding zero-JavaScript fallback
- Progressive enhancement scenarios
- Environments where JavaScript is disabled or restricted
- Server-side rendering without client-side enhancements

**Implementation Details:**
- `NoJSView.cshtml` for Pager: Same as PlainView with anchor links only
- `NoJSView.cshtml` for PageSize: Uses `<form method="get">` with submit button
- Form preserves all current query parameters as hidden inputs
- CSS classes: `.page-size-form`, `.page-size-button`

### JavaScript Mode System
The library supports multiple JavaScript frameworks through the `JavaScriptMode` enum and `js-mode` attribute:

**Available Modes:**
- `HTMX` (default): Uses HTMX for dynamic page updates
- `HTMXWithAlpine`: Combines HTMX with Alpine.js for enhanced interactivity
- `Alpine`: Uses only Alpine.js (no HTMX)
- `PlainJS`: Vanilla JavaScript with no framework dependencies
- `NoJS`: Zero JavaScript - uses standard HTML forms and links

**Backward Compatibility:**
- The `use-htmx` boolean attribute is still supported (deprecated)
- If `js-mode` is not set, the mode is derived from `use-htmx`
- `use-htmx="true"` → `JavaScriptMode.HTMX`
- `use-htmx="false"` → `JavaScriptMode.PlainJS`
- `ViewType.NoJS` automatically sets `JavaScriptMode.NoJS`

**Usage Examples:**
```html
<!-- HTMX (default) -->
<pager paging-model="Model" />
<pager paging-model="Model" js-mode="HTMX" />

<!-- HTMX with Alpine.js -->
<pager paging-model="Model" js-mode="HTMXWithAlpine" />

<!-- Pure Alpine.js -->
<pager paging-model="Model" js-mode="Alpine" />

<!-- Plain JavaScript -->
<pager paging-model="Model" js-mode="PlainJS" />
<pager paging-model="Model" use-htmx="false" />  <!-- Legacy -->

<!-- No JavaScript -->
<pager paging-model="Model" js-mode="NoJS" />
<pager paging-model="Model" view-type="NoJS" />  <!-- Also works -->
```

**Implementation Notes:**
- Views check `Model.EffectiveJSMode` to determine which JavaScript to use
- Each mode configures appropriate attributes (`hx-*`, `x-data`, `@change`, etc.)
- HTMXWithAlpine mode adds Alpine.js directives to HTMX elements
- Alpine mode handles navigation through Alpine.js event handlers
- PlainJS mode includes the vanilla JavaScript snippet
- NoJS mode uses standard HTML without any JavaScript

## Important Development Notes

### Razor SDK Project Type
This is a `Microsoft.NET.Sdk.Razor` project with embedded Razor views. Views in `Areas/Components/Views/` are compiled into the assembly.

### HTMX Integration
- HTMX is the default mode (`use-htmx="true"`)
- The TagHelpers preserve `hx-*` attributes passed to them
- When not using HTMX, consumers must include `@Html.PageSizeOnchangeSnippet()` for page size handling

### TailwindCSS Considerations
When using TailwindCSS in consuming apps, include this hidden span in `_Layout.cshtml` to preserve required classes (TailwindCSS's tree-shaking might remove classes only referenced in the embedded views):

```html
<span class="hidden btn btn-sm btn-active btn-disabled select select-primary select-sm
    text-sm text-gray-600 text-neutral-500 border rounded flex items-center
    justify-center min-w-[80px] pr-4 pt-0 mt-0 mr-2 btn-primary btn-outline
    bg-white text-black
    dark:bg-blue-500 dark:border-blue-400 dark:text-white dark:hover:bg-blue-600
    dark:bg-gray-800 dark:text-gray-300 dark:border-gray-600 dark:hover:bg-gray-700
    dark:btn-accent dark:btn-outline dark:btn-disabled dark:btn-primary dark:btn-active gap-2 whitespace-nowrap">
</span>
```

### Custom View Support
To use a custom view:
1. Set `view-type="Custom"`
2. Set `use-local-view="true"`
3. Create `Views/Components/Pager/Default.cshtml` in the consuming app
4. The ViewComponent checks if the view exists before rendering

## Localization Support

The library includes built-in localization for pagination text:

### Supported Languages
- English (default)
- Spanish (es)
- French (fr)
- German (de)
- Italian (it)
- Portuguese (pt)
- Japanese (ja)
- Chinese Simplified (zh-Hans)

### How Localization Works
1. Resource files are located in `Resources/PagingResources.[culture].resx`
2. The `PagingLocalizer` service automatically provides localized strings based on the current culture
3. Default English values are used if no localization is found
4. All text properties in `PagerViewModel` fallback to localized values if not explicitly set

### Localized Strings
- First/Previous/Next/Last page button text
- Skip forward/back ellipsis
- Page size label
- Page summary format string

## Release Process

1. Update version in `mostlylucid.pagingtaghelper.csproj`
2. Update `ReleaseNotes.txt`
3. Build in Release configuration (generates package automatically)
4. Package is output to `bin\Release\`
