# mostlylucid paging tag helper and viewcomponent.

This is a preview of this tag helper at present but *it works*. I'll be adding more documentation and samples as I go.

## Installation

You can install the package via nuget:

```bash
dotnet add package mostlylucid.pagingtaghelper
  
```

## Usage
The sample site can be found here.

https://taghelpersample.mostlylucid.net/

You can view the source for the sample site here: https://github.com/scottgal/mostlylucid.pagingtaghelper

Aditionally you can follow along with any articles on my [blog site here](https://www.mostlylucid.net/blog/category/PagingTagHelper).

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

The most basic model is as follows:

```csharp
public interface IPagingModel
{
    //    [HtmlAttributeName("page")]
    public int Page { get; set; }
   //     [HtmlAttributeName("total-items")]
    public int TotalItems { get; set; }
    //    [HtmlAttributeName("page-size")]
    public int PageSize { get; set; }
    //    [HtmlAttributeName("link-url")]
    public string LinkUrl { get; set; }
}
```

These are the required fields and can either be supplied individually to the TagHelper or as a model.

The configuration on the TagHelper is as follows. As you can see it has many options, each of which I'll add to the sample (takes some time!).



```csharp
    /// <summary>
    /// The paging model containing the pagination details (OPTIONAL).
    /// </summary>
    [HtmlAttributeName("model")]
    public IPagingModel? Model { get; set; }

    /// <summary>
    /// Whether to enable HTMX use for the pagesize component. Defaults to true.
    /// </summary>
    [HtmlAttributeName("use-htmx")] public bool UseHtmx { get; set; } = true;
    /// <summary>
    /// Optionally pass in an id for the pager component.
    /// </summary>
    [HtmlAttributeName("id")]
    public string? PagerId { get; set; }

    /// <summary>
    /// The view model for the pager component (OPTIONAL).
    /// </summary>
    [HtmlAttributeName("pagingmodel")]
    public PagerViewModel? PagerModel { get; set; } = null;

    /// <summary>
    /// Determines whether the page size selection is shown.
    /// </summary>
    [HtmlAttributeName("show-pagesize")]
    public bool ShowPageSize { get; set; } = true;
    
    /// <summary>
    /// The search term used in pagination filtering.
    /// </summary>
    [HtmlAttributeName("search-term")]
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// The base URL for pagination links.
    /// </summary>
    [HtmlAttributeName("link-url")]
    public string? LinkUrl { get; set; }

    /// <summary>
    /// The current page number.
    /// </summary>
    [HtmlAttributeName("page")]
    public int? Page { get; set; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    [HtmlAttributeName("page-size")]
    public int? PageSize { get; set; }
    
    /// <summary>
    /// Sets the view to use the local view or to use the taghelper view.
    /// </summary>

    [HtmlAttributeName("use-local-view")]
    public bool UseLocalView { get; set; } = false;
    
    /// <summary>
    /// The total number of items in the pagination set.
    /// </summary>
    [HtmlAttributeName("total-items")]
    public int? TotalItems { get; set; }

    /// <summary>
    /// The number of pages to display in the pager navigation.
    /// </summary>
    [HtmlAttributeName("pages-to-display")]
    public int PagesToDisplay { get; set; } = 5;

    /// <summary>
    /// The CSS class applied to the pager container.
    /// </summary>
    [HtmlAttributeName("css-class")]
    public string CssClass { get; set; } = "btn-group";

    /// <summary>
    /// Text for the first page navigation link.
    /// </summary>
    [HtmlAttributeName("first-page-text")]
    public string FirstPageText { get; set; } = "«";

    /// <summary>
    /// Text for the previous page navigation link.
    /// </summary>
    [HtmlAttributeName("previous-page-text")]
    public string PreviousPageText { get; set; } = "‹ Previous";

    /// <summary>
    /// Text for skipping backward in pagination.
    /// </summary>
    [HtmlAttributeName("skip-back-text")]
    public string SkipBackText { get; set; } = "..";

    /// <summary>
    /// Text for skipping forward in pagination.
    /// </summary>
    [HtmlAttributeName("skip-forward-text")]
    public string SkipForwardText { get; set; } = "..";

    /// <summary>
    /// Text for the next page navigation link.
    /// </summary>
    [HtmlAttributeName("next-page-text")]
    public string NextPageText { get; set; } = "Next ›";

    /// <summary>
    /// ARIA label for the next page navigation link.
    /// </summary>
    [HtmlAttributeName("next-page-aria-label")]
    public string NextPageAriaLabel { get; set; } = "go to next page";

    /// <summary>
    /// Text for the last page navigation link.
    /// </summary>
    [HtmlAttributeName("last-page-text")]
    public string LastPageText { get; set; } = "»";

    /// <summary>
    /// Indicates whether first and last page navigation links should be displayed.
    /// </summary>
    [HtmlAttributeName("first-last-navigation")]
    public bool FirstLastNavigation { get; set; } = true;

    /// <summary>
    /// Indicates whether skip forward/backward navigation should be enabled.
    /// </summary>
    [HtmlAttributeName("skip-forward-back-navigation")]
    public bool SkipForwardBackNavigation { get; set; } = true;

    /// <summary>
    /// Specifies the HTMX target for AJAX-based pagination.
    /// </summary>
    [HtmlAttributeName("htmx-target")]
    public string HtmxTarget { get; set; } = "";

```

This defines all the configuration options for the tag helper.

NOTE: As it stands this site uses DaisyUI with TailwindCSS; later I'll document how to use it with any CSS framework.

For Tailwind & DaisyUI I also add a piece of HTML to the _layout.cshtml file provide a hint to the tailwind processor for required classes. 

```html
<!-- Dummy Hidden Span to Preserve Tailwind Classes -->
<span class="hidden btn btn-sm btn-active btn-disabled select select-primary select-sm
        text-sm text-gray-600 text-neutral-500 border rounded flex items-center
        justify-center min-w-[80px] pr-4 pt-0 mt-0 mr-2 btn-primary btn-outline
bg-white text-black
        dark:bg-blue-500 dark:border-blue-400 dark:text-white dark:hover:bg-blue-600 
        dark:bg-gray-800 dark:text-gray-300 dark:border-gray-600 dark:hover:bg-gray-700
        dark:btn-accent dark:btn-outline dark:btn-disabled dark:btn-primary dark:btn-active gap-2 whitespace-nowrap">
</span>


```

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

As I designed this to be HTMX by default you only need to specify `ust-htmx="false"` if you AREN'T using HTMX. In that case you can use the Page Size JS snipept to handle the page size selector submission.

```csharp
@Html.PageSizeOnchangeSnippet()
```

Additionally you can select from multiple Views using the `ViewType` property, set using the ViewType attribute `view-type="Custom"`.


```csharp
    public ViewType ViewType { get; set; } = Models.ViewType.TailwindANdDaisy;
  ```

When set to Custom you are able to pass in the view name using the `UseLocalView` property / `"use-local-view="\ViewPath.cshtml"`. property.

This is handled within the `PagerViewComponent` which is used to render the pager. This is a ViewComponent that is used to render the pager. It uses the `PagerViewModel` to render the pager. 

```csharp
        var viewName = "Components/Pager/Default";

            var useLocalView = model.UseLocalView;

            return (useLocalView, model.ViewType) switch
            {
                (true, ViewType.Custom) when ViewExists(viewName) => View(viewName, model),
                (true, ViewType.Custom) when !ViewExists(viewName) => throw new ArgumentException("View not found: " + viewName),
                (false, ViewType.Bootstrap) => View("/Areas/Components/Views/Pager/BootstrapView.cshtml", model),
                (false, ViewType.Plain) => View("/Areas/Components/Views/Pager/PlainView.cshtml", model),
                (false, ViewType.TailwindANdDaisy) => View("/Areas/Components/Views/Pager/Default.cshtml", model),
                _ => View("/Areas/Components/Views/Pager/Default.cshtml", model)
            };
            
         /// <summary>
        /// Checks if a view exists in the consuming application.
        /// </summary>
        private bool ViewExists(string viewName)
        {
            var result = ViewEngine.FindView(ViewContext, viewName, false);
            return result.Success;
        }
```


## TBC
Additionally I'll write some blog posts over on my [site](https://www.mostlylucid.net) about how to use this tag helper in a real-world scenario as well as detail the thinking behind it's creation etc.

ViewComponent - one aspect of this is I use a ViewComponent in conjunction with a tag helper; this pemits me to use the tag helper in a more dynamic way (for instance you can replace the Default view with your own custom view).