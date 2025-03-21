using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using mostlylucid.pagingtaghelper.Helpers;

namespace mostlylucid.pagingtaghelper.Extensions;

public static class HtmlExtensions
{
    public static IHtmlContent PageSizeOnchangeSnippet(this IHtmlHelper helper)
    {
        var javaScript = MostlylucidSnippets.Pagesizeonchange;
        var script = helper.Raw($"<script>{javaScript}</script>");
        return script;
    }
    
    public static IHtmlContent PlainCSS(this IHtmlHelper helper)
    {
        var css = MostlylucidSnippets.PlainViewCSS;
        var script = helper.Raw($"<style>{css}</style>");
        return script;
    }
    
    public static IHtmlContent HTMXPageSizeChange(this IHtmlHelper helper)
    {
        var javaScript = MostlylucidSnippets.HTMXPageSizeChange;
        var script = helper.Raw($"<script>{javaScript}</script>");
        return script;
    }
    public static IHtmlContent PlainMinCSS(this IHtmlHelper helper)
    {
        var css = MostlylucidSnippets.PlainViewMinCSS;
        var script = helper.Raw($"<style>{css}</style>");
        return script;
    }
}