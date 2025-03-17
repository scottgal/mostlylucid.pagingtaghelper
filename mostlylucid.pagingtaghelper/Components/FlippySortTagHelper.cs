using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace mostlylucid.pagingtaghelper.Components;

[HtmlTargetElement("sortable-header")]
public class SortableHeaderTagHelper(IUrlHelperFactory urlHelperFactory) : TagHelper
{
    
    private IUrlHelper Url => urlHelperFactory.GetUrlHelper(ViewContext);
    [HtmlAttributeName("hx-controller")]
    [AspMvcController] // Enables IntelliSense for controller names
    public string? HXController { get; set; }

    [HtmlAttributeName("hx-action")]
    [AspMvcAction] // Enables IntelliSense for action names
    public string? HXAction { get; set; }
    
    
    [HtmlAttributeName("action")]
    [AspMvcAction]
    public string? Action { get; set; }
    
    [HtmlAttributeName("controller")]
    [AspMvcController]
    public string? Controller { get; set; }
    
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }
    /// <summary>
    /// The column to sort by
    /// </summary>
    [HtmlAttributeName("column")] public string Column { get; set; } = string.Empty;

    /// <summary>
    /// Whether to auto-append any query string parameters
    /// </summary>
    [HtmlAttributeName("auto-append-querystring")] public bool AutoAppend { get; set; } = true;
    
    // <summary>
    /// Whether to use htmx ; specifcally used to set hx-vals
    /// </summary>
    [HtmlAttributeName("use-htmx")] public bool UseHtmx { get; set; } = true;

    /// <summary>
    /// The currently set order by column
    /// </summary>
    [HtmlAttributeName("current-order-by")]
    public string? CurrentOrderBy { get; set; }

    /// <summary>
    /// Sort direction, true for descending, false for ascending
    /// </summary>
    [HtmlAttributeName("descending")] public bool Descending { get; set; }

    
    /// <summary>
    ///  CSS class for the chevron up icon
    /// </summary>
    [HtmlAttributeName("chevron-up-class")]
    public string? ChevronUpClass { get; set; }
    
    /// <summary>
    ///  CSS class for the chevron down icon
    /// </summary>

    [HtmlAttributeName("chevron-down-class")]
    public string? ChevronDownClass { get; set; }
    
    /// <summary>
    /// The CSS class for the chevron when unsorted
    /// </summary>
    
    [HtmlAttributeName("chevron-unsorted-class")]
    public string? ChevronUnsortedClass { get; set; }

    /// <summary>
    /// The CSS class to use for the tag.
    /// </summary>
    [HtmlAttributeName("tag-class")] public string? TagClass { get; set; }


    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        // Determine if currently sorted and new sort order
        var isSorted = CurrentOrderBy == Column;
        var newDescending = isSorted && !Descending;

        // Icon based on sorting state
        var iconClass = isSorted
            ? Descending ? ChevronDownClass ?? "bx bx-sm  bx-chevron-down" : ChevronUpClass ?? "bx bx-sm  bx-chevron-up"
            : ChevronUnsortedClass ?? "bx bx-sm  bx-sort-alt-2";

        // Set tag to anchor (<a>)
        output.TagName = "a";
        output.TagMode = TagMode.StartTagAndEndTag;

      
    
        // Default href if not specified
        if (!output.Attributes.ContainsName("href") && string.IsNullOrEmpty(Controller) && string.IsNullOrEmpty(Action))
        {
            output.Attributes.SetAttribute("href", "#");
        }
        else
        {
            // Add query string parameters
            AddQueryStringParameters(output, newDescending);
        }

        if (UseHtmx)
        {
            // Set hx-vals dynamically
            var hxValsJson = JsonSerializer.Serialize(new { orderBy = Column, descending = newDescending });
            output.Attributes.SetAttribute("hx-vals", hxValsJson);
        }
        
        // Ensure necessary CSS classes
        var existingClass = output.Attributes.ContainsName("class")
            ? output.Attributes["class"].Value.ToString()
            : string.Empty;

        var tagClass = TagClass ?? "cursor-pointer flex items-center gap-2 ";
        var finalClass = $"{tagClass} {existingClass}".Trim();
        output.Attributes.SetAttribute("class", finalClass);

        output.Attributes.RemoveAll("column");
        output.Attributes.RemoveAll("current-order-by");
        output.Attributes.RemoveAll("descending");
        output.Attributes.RemoveAll("chevron-up-class");
        output.Attributes.RemoveAll("chevron-down-class");
        output.Attributes.RemoveAll("chevron-unsorted-class");
        output.Attributes.RemoveAll("tag-class");
        output.Attributes.RemoveAll("auto-append");
        output.Attributes.RemoveAll("use-htmx");
        
        // Get child content explicitly (label inside the tag)
        var childContent = await output.GetChildContentAsync();
        var labelText = childContent.IsEmptyOrWhiteSpace
            ? Column
            : childContent.GetContent().Trim();

        // Set final content explicitly
        output.Content.SetHtmlContent($"{labelText} <i class=\"{iconClass}\"></i>");
    }

    private void AddQueryStringParameters(TagHelperOutput output, bool newDescending)
    {
        string? href = "";

        // If Controller and Action are provided, generate URL dynamically
        if (!string.IsNullOrEmpty(Controller) && !string.IsNullOrEmpty(Action))
        {
            href = Url.ActionLink(Action, Controller);
          
        }
        else if (output.Attributes.ContainsName("href")) // If href is manually set, use it
        {
            href = output.Attributes["href"].Value?.ToString() ?? "";
        }
        if(string.IsNullOrEmpty(href)) throw new ArgumentException("No href was provided or could be generated");

        // If AutoAppend is false or href is still empty, don't modify anything
        if (!AutoAppend || string.IsNullOrWhiteSpace(href))
        {
            return;
        }

        // Parse the existing URL to append query parameters
        var queryStringBuilder = QueryString.Empty
            .Add("orderBy", Column)
            .Add("descending", newDescending.ToString().ToLowerInvariant());

        // Preserve existing query parameters from the current request
        foreach (var key in ViewContext.HttpContext.Request.Query.Keys)
        {
            var keyLower = key.ToLowerInvariant();
            if (keyLower != "orderby" && keyLower != "descending") // Avoid duplicating orderBy params
            {
             queryStringBuilder=   queryStringBuilder.Add(key, ViewContext.HttpContext.Request.Query[key]!);
            }
        }
href+= queryStringBuilder.ToString();
        
        // Remove old href and set the new one with the appended parameters
        output.Attributes.RemoveAll("href");
        output.Attributes.SetAttribute("href", href);
    }

}