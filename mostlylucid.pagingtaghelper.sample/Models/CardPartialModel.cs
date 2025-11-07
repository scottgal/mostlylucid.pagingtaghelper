using JetBrains.Annotations;

namespace mostlylucid.pagingtaghelper.sample.Models;

public class CardPartialModel
{
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    [AspMvcController]
    public string Controller { get; set; }
    
    [AspMvcAction]
    public string Action { get; set; }


    public string BackgroundColor { get; set; } = "";

    public string ButtonColor { get; set; } = "";
}


public class CardSectionModel
{
    public string Title { get; set; } = "";
    public List<CardPartialModel> Cards { get; set; } = new();
}