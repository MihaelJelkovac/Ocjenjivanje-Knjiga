using Lab5.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers;

[Route("[controller]")]
[Route("pretraga")]
public class SearchController : Controller
{
    private readonly IGlobalSearchService _searchService;

    public SearchController(IGlobalSearchService searchService)
    {
        _searchService = searchService;
    }

    [AllowAnonymous]
    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index(string? q)
    {
        var results = await _searchService.SearchAllAsync(q);
        return View(results);
    }
}
