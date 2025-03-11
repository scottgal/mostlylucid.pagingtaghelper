# mostlylucid paging tag helper and viewcomponent.

This is a preview of this tag helper at present but *it works*. I'll be adding more documentation and samples as I go.

## Installation

You can install the package via nuget:

```bash
dotnet add package mostlylucid.pagingtaghelper
  
```

## Usage
I am currently building a sample porject here (I'll host the sample site later). 
You can view the source for the sample site here:

- [Basic use without HTMX](https://github.com/scottgal/mostlylucid.pagingtaghelper/blob/main/mostlylucid.pagingtaghelper.sample/Views/Home/BasicWithModel.cshtml) - this sample shoows the very basic usage of the tag helper. It disables HTMX  using the `use-htmx` property set to false. 
```html
<paging model="Model"
        use-htmx="false"
        class="mt-4">
</paging>
```

It also uses the JavaScript snippet below to handle the form submission:

```csharp
@Html.PageSizeOnchangeSnippet()
```
This can be added anywhere.

It takes a `PagerViewModel` which contains the following:


```csharp
using mostlylucid.pagingtaghelper.Models;

namespace mostlylucid.pagingtaghelper.Components;

public class PagerViewModel
{
    public IPagingModel? Model { get; set; }

    public bool UseLocalView { get; set; } = false;
    
    public bool UseHtmx { get; set; } = true;

    public string? PagerId { get; set; }
    public bool ShowPageSize { get; set; } = true;
        
    public string? SearchTerm { get; set; }
    // Required properties
    public string? LinkUrl { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public int? TotalItems { get; set; }

    // Optional properties with defaults (using DaisyUI/Tailwind classes)
    public int PagesToDisplay { get; set; } = 5;
    public string CssClass { get; set; } = "btn-group";  // DaisyUI grouping style
    public string FirstPageText { get; set; } = "«";
    public string PreviousPageText { get; set; } = "‹ Previous";
    public string SkipBackText { get; set; } = "..";
    public string SkipForwardText { get; set; } = "..";
    public string NextPageText { get; set; } = "Next ›";
    public string NextPageAriaLabel { get; set; } = "go to next page";
    public string LastPageText { get; set; } = "»";
    public bool FirstLastNavigation { get; set; } = true;
    public bool SkipForwardBackNavigation { get; set; } = true;

    // Calculated value
    public int TotalPages { get; set; }

    // Optional htmx integration:
    // If set (e.g. "#content"), pagination links will include htmx attributes.
    public string HtmxTarget { get; set; } = "";
}
```

This defines all the configuration options for the tag helper.

NOTE: As it stands this site uses DaisyUI with TailwindCSS; later I'll document how to use it with any CSS framework.

- Use with HTMX - this sample shows how to use the tag helper with HTMX. It uses the same `PagerViewModel` as the basic sample but with the `UseHtmx` property set to `true`.

```html
<paging hx-boost="true"
        class="mt-4"
        hx-indicator="#loading-modal"
        hx-target="#list"
        hx-swap="show:none"
        hx-headers='{"pagerequest": "true"}'
        model="@Model">
</paging>

```

The tag helper is designed to preserve the `hx` attributes and these cascade into the tag helper. The form submission is then handled using HTMX. 

## TBC
Additionally I'll write some blog posts over on my [site](https://www.mostlylucid.net) about how to use this tag helper in a real-world scenario as welll as detail the thinking behind it's creation etc.

ViewComponent - one aspect of this is I use a ViewComponent in conjuntion with a tag helper; this pemits me to use the tag helper in a more dynamic way (for instance you can replace the Default view with your own custom view).