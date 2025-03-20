namespace mostlylucid.pagingtaghelper.Extensions;

using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

public static class QueryStringExtensions
{
    public static string BuildPagedUrl(this HttpContext context, string baseUrl, int page, int pageSize, 
        string? searchTerm = null, string? orderBy = null, bool? descending = null, params string[] excludeKeys)
    {
        // Convert current query parameters into a dictionary
        var query = context.Request.Query.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

        // Always override page & pageSize
        query["page"] = page.ToString();
        query["pageSize"] = pageSize.ToString();

        // Add optional search term (avoid duplicates)
        if (!string.IsNullOrEmpty(searchTerm))
            query["search"] = searchTerm;

        // Add optional sorting parameters
        if (!string.IsNullOrEmpty(orderBy))
            query["orderBy"] = orderBy;

        if (descending != null)
            query["descending"] = descending.Value.ToString().ToLower();

        // Remove any excluded parameters
        foreach (var key in excludeKeys)
            query.Remove(key);

        // Rebuild the query string with proper encoding
        var queryString = string.Join("&", query.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        return $"{baseUrl}?{queryString}";
    }
}