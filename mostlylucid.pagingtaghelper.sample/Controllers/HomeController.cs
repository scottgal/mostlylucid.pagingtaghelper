using System.Diagnostics;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using mostlylucid.pagingtaghelper.Extensions;
using mostlylucig.pagingtaghelper.sample.Models;
using mostlylucig.pagingtaghelper.sample.Services;
using mostlylucid.pagingtaghelper.Models;

namespace mostlylucig.pagingtaghelper.sample.Controllers;

public class HomeController(DataFakerService dataFakerService, ILogger<HomeController> logger) : Controller
{




    public IActionResult Index()
    {
        
            var sectionModels = new List<CardSectionModel>
            {
                    new CardSectionModel()
                    {
                       Title= "Pagination",
                       Cards=
                       {
                           
                           new CardPartialModel
                           {
                               Title = "Basic Pagination with Model",
                               Description = "Demonstrates basic paging using a model to manage data.",
                               Controller = "Home",
                               Action = "BasicWithModel"
                           },
                           new CardPartialModel
                           {
                               Title = "HTMX Integration",
                               Description = "Demonstrates how to use HTMX for AJAX-based paging without full page reloads.",
                               Controller = "Home",
                               Action = "BasicHtmxWithModel"
                           },
                           new CardPartialModel
                           {
                               Title = "Search With HTMX",
                               Description = "This shows how to use a search function with HTMX.",
                               Controller = "Home",
                               Action = "SearchWithHtmx"
                           },
                           new CardPartialModel
                           {
                               Title = "Plain CSS",
                               Description = "This demonstrates using 'Plain' CSS which is injected into the view.",
                               Controller = "Home",
                               Action = "PlainView"
                           }
                       },
                       
                       
                    },
                    new CardSectionModel()
                    {
                       Title="Flippy Headers",
                          Cards=
                          {
                              new CardPartialModel
                              {
                                  Title = "Page Sort",
                                  Description = "This demonstrates the use of the Header Tag Helper.",
                                  Controller = "Home",
                                  Action = "PageSortTagHelper"
                              },
                              new CardPartialModel
                              {
                                  Title = "Page Sort No HTMX",
                                  Description = "This demonstrates the use of the Header Tag Helper Without HTMX.",
                                  Controller = "Home",
                                  Action = "PageSortTagHelperNoHtmx"
                              },
                          }
                    },
                    new CardSectionModel()
                    {
                        Title="Page Size",
                        Cards =
                        {
                        new CardPartialModel
                        {
                            Title = "Page Size With HTMX",
                            Description = "This demonstrates the use of the Page Size Tag Helper With HTMX.",
                            Controller = "Home",
                            Action = "PageSizeWithHtmx"
                        },
                        {
                            new CardPartialModel
                            {
                                Title = "Page Size Without HTMX",
                                Description = "This demonstrates the use of the Page Size Tag Helper Without HTMX.",
                                Controller = "Home",
                                Action = "PageSizeNoHtmx"
                            }
                        }
                    }
                    
                }
            };
        
            return View(sectionModels);
        }
    



    
    public async Task<IActionResult> PlainView(int page = 1,int pageSize = 10)
    {
        var pagingModel = await GenerateModel(page, pageSize);
        pagingModel.ViewType = ViewType.Plain;
        return View(pagingModel);
    }
    
    [Route("BasicWithModel")]

    public async Task<IActionResult> BasicWithModel(int page = 1,int pageSize = 10)
    {
        var pagingModel = await GenerateModel(page, pageSize);

        return View(pagingModel);
    }


    [Route("BasicHtmxWithModel")]

    public async  Task<IActionResult> BasicHtmxWithModel(int page = 1,int pageSize = 10)
    {
  
        var pagingModel = await GenerateModel(page, pageSize);
        if (Request.IsHtmxBoosted() || Request.IsHtmx())
        {
            return PartialView("_BasicHtmxWithModelList", pagingModel);
        }
        return View(pagingModel);
    }

    
    [Route("SearchWithHtmx")]
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

    [Route("PageSortTagHelper")]
    public async Task<IActionResult> PageSortTagHelper(string? search, int pageSize = 10, int page = 1, string? orderBy = "", bool descending = false)
    {
        var pagingModel = await SortResults(search, pageSize, page, orderBy, descending);

        if (Request.IsHtmxBoosted() || Request.IsHtmx())
        {
            return PartialView("_PageSortTagHelper", pagingModel);
        }
        return View("PageSortTagHelper", pagingModel);
    }
    
    [Route("PageSortTagHelperNoHtmx")]
    public async Task<IActionResult> PageSortTagHelperNoHtmx(string? search, int pageSize = 10, int page = 1, string? orderBy = "", bool descending = false)
    {
        var pagingModel = await SortResults(search, pageSize, page, orderBy, descending);

        return View("PageSortTagHelperNoHtmx", pagingModel);
    }

    [Route("PageSizeWithHTMX")]
    public async Task<IActionResult> PageSizeWithHtmx(int pageSize = 10)
    {
        var pagingModel = await GenerateModel(1, pageSize, 1234);
        if (Request.IsHtmxBoosted() || Request.IsHtmx())
        {
            return PartialView("_ResultsList", pagingModel);
        }
        return View("PageSizeWithHtmx", pagingModel);
        
    }
    
    [Route("PageSizeNoHTMX")]
    public async Task<IActionResult> PageSizeNoHtmx(int pageSize = 10)
    {
        var pagingModel = await GenerateModel(1, pageSize, 1234);
        return View("PageSizeNoHtmx", pagingModel);
        
    }

    
    private async Task<OrderedPagingViewModel> SortResults(string? search, int pageSize, int page, string? orderBy, bool descending)
    {
        search = search?.Trim().ToLowerInvariant();
        var fakeModel = await dataFakerService.GenerateData(1000);
        var results = new List<FakeDataModel>();

        if (!string.IsNullOrEmpty(search))
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

        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            results = results.OrderByField(orderBy, descending).ToList();
        }

        var pagingModel = new OrderedPagingViewModel();
        pagingModel.TotalItems = results.Count();
        pagingModel.Page = page;
        pagingModel.SearchTerm = search;
        pagingModel.PageSize = pageSize;
        pagingModel.Data = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        pagingModel.OrderBy = orderBy;
        pagingModel.Descending = descending;
        return pagingModel;
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    
    
    private async Task<PagingViewModel> GenerateModel(int page, int pageSize, int totalItems = 256)
    {
        var fakeModel =await dataFakerService.GenerateData(totalItems);
        var pagingModel = new PagingViewModel();
        pagingModel.TotalItems = fakeModel.Count();
        pagingModel.Page = page;
        pagingModel.PageSize = pageSize;
        pagingModel.Data = fakeModel.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return pagingModel;
    }
    
}   