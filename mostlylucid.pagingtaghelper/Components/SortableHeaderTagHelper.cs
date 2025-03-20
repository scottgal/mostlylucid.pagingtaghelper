using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace mostlylucid.pagingtaghelper.Components;

/// <summary>
/// A tag helper that renders a sortable column header for tables.
/// </summary>
[HtmlTargetElement("sortable-header")]
public class SortableHeaderTagHelper(IUrlHelperFactory urlHelperFactory) : TagHelper
{
    private IUrlHelper Url => urlHelperFactory.GetUrlHelper(ViewContext);

    [HtmlAttributeName("hx-controller")]
    [AspMvcController]
    public string? HXController { get; set; }

    [HtmlAttributeName("hx-action")]
    [AspMvcAction]
    public string? HXAction { get; set; }

    [HtmlAttributeName("action")]
    [AspMvcAction]
    public string? Action { get; set; }

    [HtmlAttributeName("controller")]
    [AspMvcController]
    public string? Controller { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    [HtmlAttributeName("column")]
    public string Column { get; set; } = string.Empty;

    [HtmlAttributeName("auto-append-querystring")]
    public bool AutoAppend { get; set; } = true;

    [HtmlAttributeName("use-htmx")]
    public bool UseHtmx { get; set; } = true;

    [HtmlAttributeName("current-order-by")]
    public string? CurrentOrderBy { get; set; }

    [HtmlAttributeName("descending")]
    public bool Descending { get; set; }

    [HtmlAttributeName("chevron-up-class")]
    public string? ChevronUpClass { get; set; }

    [HtmlAttributeName("chevron-down-class")]
    public string? ChevronDownClass { get; set; }

    [HtmlAttributeName("chevron-unsorted-class")]
    public string? ChevronUnsortedClass { get; set; }

    [HtmlAttributeName("tag-class")]
    public string? TagClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        // Determine if currently sorted and new sort order
        var isSorted = CurrentOrderBy == Column;
        var newDescending = isSorted && !Descending;

        // Determine icon class based on sort state
        var iconClass = isSorted
            ? (Descending ? ChevronDownClass ?? "bx bx-sm bx-chevron-down" : ChevronUpClass ?? "bx bx-sm bx-chevron-up")
            : ChevronUnsortedClass ?? "bx bx-sm bx-sort-alt-2";

        // Set tag to anchor (<a>)
        output.TagName = "a";
        output.TagMode = TagMode.StartTagAndEndTag;

        // Generate the sorting link
        string href = GenerateSortingLink(output, newDescending);

        output.Attributes.SetAttribute("href", href);

        if (UseHtmx)
        {
            var hxValsJson = JsonSerializer.Serialize(new { orderBy = Column, descending = newDescending });
            output.Attributes.SetAttribute("hx-vals", hxValsJson);
        }

        // Ensure necessary CSS classes
        var existingClass = output.Attributes.ContainsName("class")
            ? output.Attributes["class"]?.Value?.ToString() ?? string.Empty
            : string.Empty;

        var finalClass = $"{TagClass ?? "cursor-pointer flex items-center gap-2"} {existingClass}".Trim();
        output.Attributes.SetAttribute("class", finalClass);

        // Remove unnecessary attributes
        string[] attributesToRemove = {
            "column", "current-order-by", "descending", "chevron-up-class",
            "chevron-down-class", "chevron-unsorted-class", "tag-class",
            "auto-append-querystring", "use-htmx"
        };

        foreach (var attr in attributesToRemove)
        {
            if (output.Attributes.ContainsName(attr))
            {
                output.Attributes.RemoveAll(attr);
            }
        }

        // Set final content with icon
        var childContent = await output.GetChildContentAsync();
        var labelText = childContent.IsEmptyOrWhiteSpace
            ? Column
            : childContent.GetContent().Trim();

        output.Content.SetHtmlContent($"{labelText} <i class=\"{iconClass}\"></i>");
    }

    private string GenerateSortingLink(TagHelperOutput output, bool newDescending)
    {
        string href = string.Empty;

        if (!string.IsNullOrEmpty(Controller) && !string.IsNullOrEmpty(Action))
        {
            href = Url.ActionLink(Action, Controller) ?? "";
        }
        else if (output.Attributes.ContainsName("href"))
        {
            href = output.Attributes["href"]?.Value?.ToString() ?? "";
        }

        if (string.IsNullOrEmpty(href))
        {
            throw new ArgumentException("No href was provided or could be generated.");
        }

        if (AutoAppend && !string.IsNullOrWhiteSpace(href))
        {
            var queryStringBuilder = QueryString.Empty
                .Add("orderBy", Column)
                .Add("descending", newDescending.ToString().ToLowerInvariant());

            foreach (var key in ViewContext.HttpContext.Request.Query.Keys)
            {
                var keyLower = key.ToLowerInvariant();
                if (keyLower != "orderby" && keyLower != "descending")
                {
                    queryStringBuilder = queryStringBuilder.Add(key, ViewContext.HttpContext.Request.Query[key]!);
                }
            }
            href += queryStringBuilder.ToString();
        }

        return href;
    }
}