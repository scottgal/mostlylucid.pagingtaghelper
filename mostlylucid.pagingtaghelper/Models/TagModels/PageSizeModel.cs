namespace mostlylucid.pagingtaghelper.Models.TagModels;

public class PageSizeModel : BaseTagModel
{
    public bool UseHtmx { get; set; } = true;
    public string? LinkUrl { get; set; }

    
}