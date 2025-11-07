namespace mostlylucid.pagingtaghelper.Models;

/// <summary>
/// Specifies the JavaScript framework/mode used for client-side interactivity.
/// </summary>
public enum JavaScriptMode
{
    /// <summary>
    /// Uses HTMX for dynamic updates (default for backward compatibility).
    /// </summary>
    HTMX = 0,

    /// <summary>
    /// Uses HTMX with Alpine.js for enhanced interactivity.
    /// </summary>
    HTMXWithAlpine = 1,

    /// <summary>
    /// Uses only Alpine.js for client-side interactivity.
    /// </summary>
    Alpine = 2,

    /// <summary>
    /// Uses plain JavaScript (no framework dependencies).
    /// </summary>
    PlainJS = 3,

    /// <summary>
    /// No JavaScript - uses standard HTML forms and links only.
    /// </summary>
    NoJS = 4
}
