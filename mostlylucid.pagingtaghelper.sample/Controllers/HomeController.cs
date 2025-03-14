using System.Diagnostics;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using mostlylucig.pagingtaghelper.sample.Models;
using mostlylucig.pagingtaghelper.sample.Services;
using  mostlylucid.pagingtaghelper.Helpers;
using mostlylucid.pagingtaghelper.Models;

namespace mostlylucig.pagingtaghelper.sample.Controllers;

public class HomeController(DataFakerService dataFakerService, ILogger<HomeController> logger) : Controller
{




    public IActionResult Index()
    {
        return View();
    }


    
    public async Task<IActionResult> PlainView(int page = 1,int pageSize = 10)
    {
        var pagingModel = await GenerateModel(page, pageSize);
        pagingModel.ViewType = ViewType.Plain;
        return View(pagingModel);
    }

    public async Task<IActionResult> BasicWithModel(int page = 1,int pageSize = 10)
    {
        var pagingModel = await GenerateModel(page, pageSize);

        return View(pagingModel);
    }



    public async  Task<IActionResult> BasicHtmxWithModel(int page = 1,int pageSize = 10)
    {
  
        var pagingModel = await GenerateModel(page, pageSize);
        if (Request.IsHtmxBoosted() || Request.IsHtmx())
        {
            return PartialView("_BasicHtmxWithModelList", pagingModel);
        }
        return View(pagingModel);
    }

    
    public async Task<IActionResult> SearchWithHtmx(string? search, int pageSize = 10,int page = 1)
    {
        search = search?.Trim().ToLowerInvariant();
        var fakeModel =await dataFakerService.GenerateData(1000);
        var results = new List<FakeDataModel>();
        
        if(!string.IsNullOrEmpty(search))
         results = fakeModel.Where(x => x.Name.ToLowerInvariant().Contains(search)
                                           || x.Description.ToLowerInvariant().Contains(search) ||
                                           x.CompanyAddress.ToLowerInvariant().Contains(search)
                                           || x.CompanyEmail.ToLowerInvariant().Contains(search)
                                           || x.CompanyCity.ToLowerInvariant().Contains(search)
                                           || x.CompanyCountry.ToLowerInvariant().Contains(search)
                                           || x.CompanyPhone.ToLowerInvariant().Contains(search)).ToList();
        else
        {
            results = fakeModel.ToList();
        }
        
        var pagingModel = new SearchPagingViewModel();
        pagingModel.TotalItems = results.Count();
        pagingModel.Page = page;
        pagingModel.SearchTerm = search;
        pagingModel.PageSize = pageSize;
        pagingModel.Data = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        
        if (Request.IsHtmxBoosted() || Request.IsHtmx())
        {
            return PartialView("_SearchWithHtmxPartial", pagingModel);
        }
        return View("SearchWithHtmx",pagingModel);
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    private async Task<PagingViewModel> GenerateModel(int page, int pageSize)
    {
        var fakeModel =await dataFakerService.GenerateData(256);
        var pagingModel = new PagingViewModel();
        pagingModel.TotalItems = fakeModel.Count();
        pagingModel.Page = page;
        pagingModel.PageSize = pageSize;
        pagingModel.Data = fakeModel.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return pagingModel;
    }
}