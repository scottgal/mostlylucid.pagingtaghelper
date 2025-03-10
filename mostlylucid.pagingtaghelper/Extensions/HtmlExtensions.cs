﻿using Microsoft.AspNetCore.Html;
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
}