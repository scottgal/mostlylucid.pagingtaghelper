namespace mostlylucid.pagingtaghelper.Models.TagModels;

public class PageSizeModel : BaseTagModel
{
    public bool UseHtmx { get; set; } = true;
    public string? LinkUrl { get; set; }

    /// <summary>
    /// Gets the effective JavaScript mode, considering backward compatibility.
    /// </summary>
    public JavaScriptMode EffectiveJSMode
    {
        get
        {
            // If explicitly set, use it
            if (JSMode.HasValue)
                return JSMode.Value;

            // Backward compatibility: NoJS ViewType
            if (ViewType == ViewType.NoJS)
                return JavaScriptMode.NoJS;

            // Backward compatibility: UseHtmx property
            return UseHtmx ? JavaScriptMode.HTMX : JavaScriptMode.PlainJS;
        }
    }
}