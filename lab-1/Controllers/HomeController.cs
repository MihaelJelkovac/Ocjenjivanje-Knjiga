using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lab1.Models;
using Lab1.Services;

namespace Lab1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBookCatalogService _bookCatalogService;

    public HomeController(ILogger<HomeController> logger, IBookCatalogService bookCatalogService)
    {
        _logger = logger;
        _bookCatalogService = bookCatalogService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _bookCatalogService.GetDashboardAsync();
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
