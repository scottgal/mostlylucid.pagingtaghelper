using System.ComponentModel.DataAnnotations;

namespace mostlylucid.pagingtaghelper.sample.Models;

public class FakeDataModel
{
    [Display(Name = "Id")]
    public int Id { get; set; }
    [Display(Name = "Name")]
    public string Name { get; set; }
    
    [Display(Name = "Description")]
    public string Description { get; set; }
    
    [Display(Name = "Company Name")]
    public string CompanyName { get; set; }
    
    [Display(Name = "Address")]
    public string CompanyAddress { get; set; }
    
    [Display(Name = "City")]
    public string CompanyCity { get; set; }
    
    [Display(Name = "Country")]
    public string CompanyCountry { get; set; }
    
    [Display(Name = "Email")]
    public string CompanyEmail { get; set; }
    
    [Display(Name = "Phone")]
    public string CompanyPhone { get; set; }
    
}