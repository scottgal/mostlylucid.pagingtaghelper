namespace mostlylucid.pagingtaghelper.Models;

/// <summary>
/// Interface for models that support continuation token-based pagination
/// (e.g., Cosmos DB, Azure Table Storage, AWS DynamoDB).
/// </summary>
public interface IContinuationPagingModel
{
    /// <summary>
    /// The continuation token for the next page of results.
    /// </summary>
    string? NextPageToken { get; set; }

    /// <summary>
    /// Indicates whether there are more results available.
    /// </summary>
    bool HasMoreResults { get; set; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    int PageSize { get; set; }

    /// <summary>
    /// Optional: Current page number (1-based) for display purposes.
    /// Not required but helps with UI.
    /// </summary>
    int CurrentPage { get; set; }

    /// <summary>
    /// Optional: The ViewType for rendering (default: TailwindAndDaisy).
    /// </summary>
    ViewType ViewType { get; set; }

    /// <summary>
    /// Optional: Dictionary storing page tokens for backward navigation.
    /// Key = page number, Value = token to get to that page.
    /// This enables "quick" navigation by storing token history.
    /// </summary>
    Dictionary<int, string>? PageTokenHistory { get; set; }
}
