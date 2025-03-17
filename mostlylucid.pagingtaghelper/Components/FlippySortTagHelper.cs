namespace mostlylucid.pagingtaghelper.Components;

using System.Text.Json;
using Microsoft.AspNetCore.Razor.TagHelpers;


[HtmlTargetElement("sortable-header")]
public class SortableHeaderTagHelper : TagHelper
{
    [HtmlAttributeName("column")] public string Column { get; set; } = string.Empty;

    [HtmlAttributeName("current-order-by")]
    public string? CurrentOrderBy { get; set; }

    [HtmlAttributeName("descending")] public bool Descending { get; set; }

    [HtmlAttributeName("chevron-up-class")]
    public string? ChevronUpClass { get; set; }

    [HtmlAttributeName("chevron-down-class")]
    public string? ChevronDownClass { get; set; }

    [HtmlAttributeName("chevron-unsorted-class")]
    public string? ChevronUnsortedClass { get; set; }

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
        if (!output.Attributes.ContainsName("href")) output.Attributes.SetAttribute("href", "#");

        // Set hx-vals dynamically
        var hxValsJson = JsonSerializer.Serialize(new { orderBy = Column, descending = newDescending });
        output.Attributes.SetAttribute("hx-vals", hxValsJson);

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
        // Get child content explicitly (label inside the tag)
        var childContent = await output.GetChildContentAsync();
        var labelText = childContent.IsEmptyOrWhiteSpace
            ? Column
            : childContent.GetContent().Trim();

        // Set final content explicitly
        output.Content.SetHtmlContent($"{labelText} <i class=\"{iconClass}\"></i>");
    }
}