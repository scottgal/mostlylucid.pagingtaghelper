# Creating Custom Views

This guide explains how to create custom views for the Pager, PageSize, ContinuationPager, and SortableHeader TagHelpers. Custom views allow you to completely control the HTML markup and styling while still benefiting from the library's logic and functionality.

## Table of Contents

- [Overview](#overview)
- [View Component Architecture](#view-component-architecture)
- [Creating Custom Views](#creating-custom-views)
- [View Models Reference](#view-models-reference)
- [Examples](#examples)
- [Best Practices](#best-practices)

## Overview

The library uses a ViewComponent pattern where TagHelpers delegate rendering to ViewComponents. Each ViewComponent selects a view based on the `ViewType` setting:

- **Default**: TailwindCSS + DaisyUI
- **TailwindView**: Pure TailwindCSS
- **BootstrapView**: Bootstrap styling
- **PlainView**: Plain CSS
- **NoJSView**: Zero JavaScript
- **Custom**: Your custom view

## View Component Architecture

### How It Works

1. **TagHelper** processes attributes and builds a ViewModel
2. **ViewComponent** receives the ViewModel and selects a view
3. **View** renders HTML using the ViewModel data

### Folder Structure

```
YourApp/
  Views/
    Components/
      Pager/
        Default.cshtml           ← Your custom pager view
      PageSize/
        Default.cshtml           ← Your custom page size view
      ContinuationPager/
        Default.cshtml           ← Your custom continuation pager view
```

Note: SortableHeader doesn't use views (it's rendered directly by the TagHelper).

## Creating Custom Views

### Step 1: Enable Custom View

Set `use-local-view="true"` and `view-type="Custom"` on the TagHelper:

```html
<paging model="Model"
        view-type="Custom"
        use-local-view="true" />
```

### Step 2: Create View File

Create a Razor view in the appropriate location:

**For Pager:**
```
Views/Components/Pager/Default.cshtml
```

**For PageSize:**
```
Views/Components/PageSize/Default.cshtml
```

**For ContinuationPager:**
```
Views/Components/ContinuationPager/Default.cshtml
```

### Step 3: Define the Model

Start your view with the model directive:

```cshtml
@model mostlylucid.pagingtaghelper.Models.TagModels.PagerViewModel
```

or

```cshtml
@model mostlylucid.pagingtaghelper.Models.TagModels.PageSizeModel
```

or

```cshtml
@model mostlylucid.pagingtaghelper.Models.TagModels.ContinuationPagerViewModel
```

### Step 4: Build Your Markup

Use the ViewModel properties to render your custom HTML.

## View Models Reference

### PagerViewModel

**Properties:**

```csharp
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
public string GetPageSummary()
```

### PageSizeModel

**Properties:**

```csharp
public int PageSize { get; set; }
public int TotalItems { get; set; }
public List<int> PageSizes { get; set; }
public string? LinkUrl { get; set; }
public bool UseHtmx { get; set; }
public JavaScriptMode EffectiveJSMode { get; }
public IPagingLocalizer? Localizer { get; set; }
```

### ContinuationPagerViewModel

**Properties:**

```csharp
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

// Methods
public string? GetPreviousPageToken()
public bool CanNavigatePrevious { get; }
public bool CanNavigateNext { get; }
```

## Examples

### Example 1: Custom Pager with Semantic UI

```cshtml
@model mostlylucid.pagingtaghelper.Models.TagModels.PagerViewModel

<div class="ui pagination menu">
    @if (Model.Page > 1)
    {
        <a class="item" href="@Model.LinkUrl?page=1&pageSize=@Model.PageSize">
            <i class="angle double left icon"></i>
        </a>
        <a class="item" href="@Model.LinkUrl?page=@(Model.Page - 1)&pageSize=@Model.PageSize">
            <i class="angle left icon"></i>
        </a>
    }

    @for (int i = Model.StartPage; i <= Model.EndPage; i++)
    {
        if (i == Model.Page)
        {
            <a class="active item">@i</a>
        }
        else
        {
            <a class="item" href="@Model.LinkUrl?page=@i&pageSize=@Model.PageSize">@i</a>
        }
    }

    @if (Model.Page < Model.TotalPages)
    {
        <a class="item" href="@Model.LinkUrl?page=@(Model.Page + 1)&pageSize=@Model.PageSize">
            <i class="angle right icon"></i>
        </a>
        <a class="item" href="@Model.LinkUrl?page=@Model.TotalPages&pageSize=@Model.PageSize">
            <i class="angle double right icon"></i>
        </a>
    }
</div>

@if (Model.ShowSummary)
{
    <div class="ui label">
        @Model.GetPageSummary()
    </div>
}
```

### Example 2: Custom PageSize with Material Design

```cshtml
@model mostlylucid.pagingtaghelper.Models.TagModels.PageSizeModel

<div class="mdc-select mdc-select--filled">
    <div class="mdc-select__anchor">
        <span class="mdc-select__ripple"></span>
        <span class="mdc-select__selected-text">@Model.PageSize items</span>
        <span class="mdc-select__dropdown-icon">
            <svg class="mdc-select__dropdown-icon-graphic" viewBox="7 10 10 5">
                <polygon stroke="none" fill-rule="evenodd" points="7 10 12 15 17 10"></polygon>
            </svg>
        </span>
        <span class="mdc-line-ripple"></span>
    </div>

    <div class="mdc-select__menu mdc-menu mdc-menu-surface" role="listbox">
        <ul class="mdc-list">
            @foreach (var size in Model.PageSizes)
            {
                <li class="mdc-list-item @(size == Model.PageSize ? "mdc-list-item--selected" : "")"
                    data-value="@size"
                    role="option"
                    onclick="window.location.href='@Model.LinkUrl?pageSize=@size'">
                    <span class="mdc-list-item__text">@size items</span>
                </li>
            }
        </ul>
    </div>
</div>
```

### Example 3: Custom Pager with Foundation

```cshtml
@model mostlylucid.pagingtaghelper.Models.TagModels.PagerViewModel

<ul class="pagination" role="navigation" aria-label="Pagination">
    <li class="pagination-previous @(Model.Page == 1 ? "disabled" : "")">
        @if (Model.Page > 1)
        {
            <a href="@Model.LinkUrl?page=@(Model.Page - 1)&pageSize=@Model.PageSize">
                Previous
            </a>
        }
        else
        {
            <span>Previous</span>
        }
    </li>

    @for (int i = Model.StartPage; i <= Model.EndPage; i++)
    {
        <li class="@(i == Model.Page ? "current" : "")">
            @if (i == Model.Page)
            {
                <span class="show-for-sr">You're on page</span> @i
            }
            else
            {
                <a href="@Model.LinkUrl?page=@i&pageSize=@Model.PageSize" aria-label="Page @i">@i</a>
            }
        </li>
    }

    <li class="pagination-next @(Model.Page == Model.TotalPages ? "disabled" : "")">
        @if (Model.Page < Model.TotalPages)
        {
            <a href="@Model.LinkUrl?page=@(Model.Page + 1)&pageSize=@Model.PageSize">
                Next
            </a>
        }
        else
        {
            <span>Next</span>
        }
    </li>
</ul>
```

### Example 4: Minimalist Custom Pager

```cshtml
@model mostlylucid.pagingtaghelper.Models.TagModels.PagerViewModel

<nav class="simple-pager">
    @if (Model.Page > 1)
    {
        <a href="@Model.LinkUrl?page=@(Model.Page - 1)&pageSize=@Model.PageSize" class="prev">
            ← Prev
        </a>
    }

    <span class="current">
        Page @Model.Page of @Model.TotalPages
    </span>

    @if (Model.Page < Model.TotalPages)
    {
        <a href="@Model.LinkUrl?page=@(Model.Page + 1)&pageSize=@Model.PageSize" class="next">
            Next →
        </a>
    }
</nav>

<style>
    .simple-pager {
        display: flex;
        gap: 1rem;
        align-items: center;
        justify-content: center;
        padding: 1rem 0;
    }
    .simple-pager a {
        color: #0066cc;
        text-decoration: none;
    }
    .simple-pager a:hover {
        text-decoration: underline;
    }
    .simple-pager .current {
        color: #666;
    }
</style>
```

### Example 5: Custom ContinuationPager

```cshtml
@model mostlylucid.pagingtaghelper.Models.TagModels.ContinuationPagerViewModel

<div class="continuation-nav">
    @if (Model.CanNavigatePrevious)
    {
        var prevToken = Model.GetPreviousPageToken();
        <a href="@Model.LinkUrl?pageToken=@Uri.EscapeDataString(prevToken)&currentPage=@(Model.CurrentPage - 1)&pageSize=@Model.PageSize"
           class="nav-button prev">
            <span class="icon">←</span>
            <span class="text">@Model.PreviousPageText</span>
        </a>
    }
    else
    {
        <span class="nav-button prev disabled">
            <span class="icon">←</span>
            <span class="text">@Model.PreviousPageText</span>
        </span>
    }

    @if (Model.ShowPageNumber)
    {
        <div class="page-indicator">
            <span class="badge">Page @Model.CurrentPage</span>
        </div>
    }

    @if (Model.CanNavigateNext)
    {
        <a href="@Model.LinkUrl?pageToken=@Uri.EscapeDataString(Model.NextPageToken)&currentPage=@(Model.CurrentPage + 1)&pageSize=@Model.PageSize"
           class="nav-button next">
            <span class="text">@Model.NextPageText</span>
            <span class="icon">→</span>
        </a>
    }
    else
    {
        <span class="nav-button next disabled">
            <span class="text">@Model.NextPageText</span>
            <span class="icon">→</span>
        </span>
    }
</div>
```

### Example 6: HTMX-Enhanced Custom View

```cshtml
@model mostlylucid.pagingtaghelper.Models.TagModels.PagerViewModel
@using mostlylucid.pagingtaghelper.Models
@{
    var jsMode = Model.EffectiveJSMode;
}

<div class="pager-container">
    @for (int i = Model.StartPage; i <= Model.EndPage; i++)
    {
        var pageNum = i;
        var pageUrl = $"{Model.LinkUrl}?page={pageNum}&pageSize={Model.PageSize}";

        @if (pageNum == Model.Page)
        {
            <span class="page-button active">@pageNum</span>
        }
        else if (jsMode == JavaScriptMode.HTMX && !string.IsNullOrEmpty(Model.HtmxTarget))
        {
            <button
                class="page-button"
                hx-get="@pageUrl"
                hx-target="@Model.HtmxTarget"
                hx-swap="outerHTML">
                @pageNum
            </button>
        }
        else
        {
            <a href="@pageUrl" class="page-button">@pageNum</a>
        }
    }
</div>
```

### Example 7: Accessible Custom View

```cshtml
@model mostlylucid.pagingtaghelper.Models.TagModels.PagerViewModel

<nav aria-label="Pagination Navigation">
    <ul class="pagination-list">
        @if (Model.FirstLastNavigation && Model.Page > 1)
        {
            <li>
                <a href="@Model.LinkUrl?page=1&pageSize=@Model.PageSize"
                   aria-label="Go to first page">
                    @Model.FirstPageText
                </a>
            </li>
        }

        @if (Model.Page > 1)
        {
            <li>
                <a href="@Model.LinkUrl?page=@(Model.Page - 1)&pageSize=@Model.PageSize"
                   aria-label="Go to previous page">
                    @Model.PreviousPageText
                </a>
            </li>
        }

        @for (int i = Model.StartPage; i <= Model.EndPage; i++)
        {
            <li>
                @if (i == Model.Page)
                {
                    <span class="current-page" aria-current="page" aria-label="Current page @i">
                        @i
                    </span>
                }
                else
                {
                    <a href="@Model.LinkUrl?page=@i&pageSize=@Model.PageSize"
                       aria-label="Go to page @i">
                        @i
                    </a>
                }
            </li>
        }

        @if (Model.Page < Model.TotalPages)
        {
            <li>
                <a href="@Model.LinkUrl?page=@(Model.Page + 1)&pageSize=@Model.PageSize"
                   aria-label="@Model.NextPageAriaLabel">
                    @Model.NextPageText
                </a>
            </li>
        }

        @if (Model.FirstLastNavigation && Model.Page < Model.TotalPages)
        {
            <li>
                <a href="@Model.LinkUrl?page=@Model.TotalPages&pageSize=@Model.PageSize"
                   aria-label="Go to last page">
                    @Model.LastPageText
                </a>
            </li>
        }
    </ul>

    @if (Model.ShowSummary)
    {
        <p class="sr-only" aria-live="polite">
            @Model.GetPageSummary()
        </p>
    }
</nav>
```

## Best Practices

### 1. Always Handle Edge Cases

```cshtml
@* Check for no results *@
@if (Model.TotalItems == 0)
{
    <p>No items to display.</p>
}
else
{
    @* Your pagination markup *@
}
```

### 2. Preserve Query Parameters

When building URLs, preserve existing query parameters:

```cshtml
@{
    var currentQuery = Context.Request.QueryString.Value ?? "";
    // Parse and merge with new page parameters
}
```

### 3. Use Accessibility Attributes

```html
<nav aria-label="Pagination">
    <button aria-label="Go to previous page">Previous</button>
    <span aria-current="page">5</span>
    <button aria-label="Go to next page">Next</button>
</nav>
```

### 4. Support Keyboard Navigation

```html
<a href="/page/2" tabindex="0">2</a>
```

### 5. Provide Visual Feedback

```css
.page-button:hover {
    background-color: #f0f0f0;
}
.page-button.active {
    background-color: #0066cc;
    color: white;
}
.page-button:focus {
    outline: 2px solid #0066cc;
    outline-offset: 2px;
}
```

### 6. Handle JavaScript Modes

```cshtml
@{
    var jsMode = Model.EffectiveJSMode;
}

@if (jsMode == JavaScriptMode.HTMX)
{
    <button hx-get="..." hx-target="#content">Next</button>
}
else if (jsMode == JavaScriptMode.NoJS)
{
    <a href="...">Next</a>
}
else
{
    <button onclick="navigateTo(...)">Next</button>
}
```

### 7. Test Responsiveness

```cshtml
<div class="pager-responsive">
    <!-- Desktop view -->
    <div class="hidden md:flex">
        @* Full pagination *@
    </div>

    <!-- Mobile view -->
    <div class="flex md:hidden">
        @* Simplified prev/next only *@
    </div>
</div>
```

### 8. Use Localization

```cshtml
@if (Model.Localizer != null)
{
    <span>@Model.Localizer.PreviousPageText</span>
}
else
{
    <span>@Model.PreviousPageText</span>
}
```

## Testing Your Custom View

1. **Create a test page** with various states:
   - First page
   - Middle page
   - Last page
   - Single page
   - No results

2. **Test all JavaScript modes:**
   - HTMX
   - HTMXWithAlpine
   - Alpine
   - PlainJS
   - NoJS

3. **Test accessibility:**
   - Screen reader navigation
   - Keyboard-only navigation
   - High contrast mode

4. **Test responsiveness:**
   - Mobile devices
   - Tablets
   - Desktop

## See Also

- [Pager TagHelper](pager-taghelper.md)
- [PageSize TagHelper](pagesize-taghelper.md)
- [ContinuationPager TagHelper](continuation-pager-taghelper.md)
- [JavaScript Modes Guide](javascript-modes.md)
