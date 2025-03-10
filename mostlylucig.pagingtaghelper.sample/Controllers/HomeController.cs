using System.Diagnostics;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using mostlylucig.pagingtaghelper.sample.Models;
using mostlylucig.pagingtaghelper.sample.Services;
using  mostlylucid.pagingtaghelper.Helpers;
namespace mostlylucig.pagingtaghelper.sample.Controllers;

public class HomeController(DataFakerService dataFakerService, ILogger<HomeController> logger) : Controller
{




    public IActionResult Index()
    {
        return View();
    }


    public async Task<IActionResult> BasicWithModel(int page = 1,int pageSize = 10)
    {
        var pagingModel = await GenerateModel(page, pageSize);

        return View(pagingModel);
    }



    public async  Task<IActionResult> BasicHtmxWithModel(int page = 1,int pageSize = 10)
    {
  
        var pagingModel = await GenerateModel(page, pageSize);
        if (Request.IsHtmxBoosted() && Request.IsPageRequest())
        {
            return PartialView("_BasicHtmxWithModelList", pagingModel);
        }
        return View(pagingModel);
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