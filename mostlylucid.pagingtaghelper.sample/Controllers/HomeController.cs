using System.Diagnostics;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using mostlylucid.pagingtaghelper.Extensions;
using mostlylucid.pagingtaghelper.sample.Models;
using mostlylucid.pagingtaghelper.sample.Services;
using mostlylucid.pagingtaghelper.Models;

namespace mostlylucid.pagingtaghelper.sample.Controllers;

public class HomeController(DataFakerService dataFakerService, ILogger<HomeController> logger) : Controller
{




    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByHeader = "Accept-Encoding")]
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
                           },
                           new CardPartialModel
                           {
                               Title = "Pure Tailwind",
                               Description = "Uses pure TailwindCSS without DaisyUI components.",
                               Controller = "Home",
                               Action = "TailwindView"
                           },
                           new CardPartialModel
                           {
                               Title = "No JavaScript",
                               Description = "Fully functional pagination with zero JavaScript - uses forms and standard links.",
                               Controller = "Home",
                               Action = "NoJSView"
                           }
                       },


                    },
                    new CardSectionModel()
                    {
                       Title="JavaScript Modes",
                          Cards=
                          {
                              new CardPartialModel
                              {
                                  Title = "JavaScript Framework Modes",
                                  Description = "Demonstrates all JavaScript modes: HTMX, HTMXWithAlpine, Alpine, PlainJS, and NoJS.",
                                  Controller = "Home",
                                  Action = "JavaScriptModes"
                              }
                          }
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

                },
                    new CardSectionModel()
                    {
                        Title="Continuation Pager (NoSQL)",
                        Cards =
                        {
                            new CardPartialModel
                            {
                                Title = "Continuation Token Pagination",
                                Description = "Demonstrates pagination using continuation tokens like Cosmos DB, DynamoDB, and Azure Table Storage. Supports token history for backward navigation.",
                                Controller = "Home",
                                Action = "ContinuationPager"
                            }
                        }
                    },
                    new CardSectionModel()
                    {
                        Title="Localization",
                        Cards =
                        {
                            new CardPartialModel
                            {
                                Title = "Multi-Language Support",
                                Description = "Interactive demo showing pagination in 8 languages: English, German, Spanish, French, Italian, Portuguese, Japanese, and Chinese.",
                                Controller = "Home",
                                Action = "Localization"
                            }
                        }
                    }
            };
        
            return View(sectionModels);
        }
    



    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "page", "pageSize" }, VaryByHeader = "Accept-Encoding")]
    public async Task<IActionResult> PlainView(int page = 1,int pageSize = 10)
    {
        var pagingModel = await GenerateModel(page, pageSize);
        pagingModel.ViewType = ViewType.Plain;
        return View(pagingModel);
    }

    [Route("TailwindView")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "page", "pageSize" }, VaryByHeader = "Accept-Encoding")]
    public async Task<IActionResult> TailwindView(int page = 1, int pageSize = 10)
    {
        var pagingModel = await GenerateModel(page, pageSize);
        pagingModel.ViewType = ViewType.Tailwind;
        return View(pagingModel);
    }

    [Route("NoJSView")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "page", "pageSize" }, VaryByHeader = "Accept-Encoding")]
    public async Task<IActionResult> NoJSView(int page = 1, int pageSize = 10)
    {
        var pagingModel = await GenerateModel(page, pageSize);
        pagingModel.ViewType = ViewType.NoJS;
        return View(pagingModel);
    }

    [Route("JavaScriptModes")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "page", "pageSize", "mode" }, VaryByHeader = "Accept-Encoding")]
    public async Task<IActionResult> JavaScriptModes(int page = 1, int pageSize = 10, string mode = "HTMX")
    {
        var pagingModel = await GenerateModel(page, pageSize);
        ViewBag.SelectedMode = mode;
        return View(pagingModel);
    }
    
    [Route("BasicWithModel")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "page", "pageSize" }, VaryByHeader = "Accept-Encoding")]
    public async Task<IActionResult> BasicWithModel(int page = 1,int pageSize = 10)
    {
        var pagingModel = await GenerateModel(page, pageSize);

        return View(pagingModel);
    }


    [Route("BasicHtmxWithModel")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "page", "pageSize" }, VaryByHeader = "Accept-Encoding")]
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
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "search", "page", "pageSize" }, VaryByHeader = "Accept-Encoding")]
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
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "search", "page", "pageSize", "orderBy", "descending" }, VaryByHeader = "Accept-Encoding")]
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
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "search", "page", "pageSize", "orderBy", "descending" }, VaryByHeader = "Accept-Encoding")]
    public async Task<IActionResult> PageSortTagHelperNoHtmx(string? search, int pageSize = 10, int page = 1, string? orderBy = "", bool descending = false)
    {
        var pagingModel = await SortResults(search, pageSize, page, orderBy, descending);

        return View("PageSortTagHelperNoHtmx", pagingModel);
    }

    [Route("PageSizeWithHTMX")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "pageSize" }, VaryByHeader = "Accept-Encoding")]
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
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "pageSize" }, VaryByHeader = "Accept-Encoding")]
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
    [Route("ContinuationPager")]
    public async Task<IActionResult> ContinuationPager(
        int currentPage = 1,
        int pageSize = 25,
        string? pageToken = null,
        string? tokenHistory = null)
    {
        // Simulate continuation token-based pagination (like Cosmos DB)
        const int totalItems = 500;
        var allData = await dataFakerService.GenerateData(totalItems);

        // Deserialize token history
        var history = string.IsNullOrEmpty(tokenHistory)
            ? new Dictionary<int, string>()
            : System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, string>>(tokenHistory)
              ?? new Dictionary<int, string>();

        // Simulate token-based navigation
        int startIndex = 0;
        if (!string.IsNullOrEmpty(pageToken))
        {
            // Decode the token (it's just a base64-encoded start index for this demo)
            try
            {
                var tokenBytes = Convert.FromBase64String(pageToken);
                var tokenString = System.Text.Encoding.UTF8.GetString(tokenBytes);
                startIndex = int.Parse(tokenString);
            }
            catch
            {
                startIndex = 0;
            }
        }

        var pageData = allData.Skip(startIndex).Take(pageSize).ToList();
        var hasMore = (startIndex + pageSize) < totalItems;

        // Generate next token
        string? nextToken = null;
        if (hasMore)
        {
            var nextStartIndex = startIndex + pageSize;
            var tokenString = nextStartIndex.ToString();
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(tokenString);
            nextToken = Convert.ToBase64String(tokenBytes);
        }

        // Store current token in history (with max limit)
        const int maxHistoryPages = 20; // Max pages to keep in history
        if (!string.IsNullOrEmpty(pageToken) && currentPage > 1)
        {
            history[currentPage] = pageToken;

            // Trim history if it exceeds max size (keep only recent pages)
            if (maxHistoryPages > 0 && history.Count > maxHistoryPages)
            {
                var oldestPages = history.Keys.OrderBy(k => k).Take(history.Count - maxHistoryPages);
                foreach (var oldPage in oldestPages.ToList())
                {
                    history.Remove(oldPage);
                }
            }
        }

        var viewModel = new ContinuationPagingViewModel
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
            NextPageToken = nextToken,
            HasMoreResults = hasMore,
            PageTokenHistory = history,
            Products = pageData,
            LinkUrl = "/ContinuationPager"
        };

        if (Request.IsHtmx())
        {
            return PartialView("_ContinuationPagerList", viewModel);
        }

        return View(viewModel);
    }

    [Route("Localization")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "page", "pageSize", "language" }, VaryByHeader = "Accept-Encoding")]
    public async Task<IActionResult> Localization(
        int page = 1,
        int pageSize = 10,
        string language = "en")
    {
        // Set the culture for localization
        var culture = new System.Globalization.CultureInfo(language);
        System.Globalization.CultureInfo.CurrentCulture = culture;
        System.Globalization.CultureInfo.CurrentUICulture = culture;

        var pagingModel = await GenerateModel(page, pageSize);
        ViewBag.SelectedLanguage = language;

        if (Request.IsHtmx())
        {
            return PartialView("_LocalizationContent", pagingModel);
        }

        return View(pagingModel);
    }

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