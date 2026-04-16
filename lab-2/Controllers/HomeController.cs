using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lab2.Models;
using Lab2.Services;
using Lab2.ViewModels;

namespace Lab2.Controllers;

public class HomeController : Controller
{
    private readonly IBookCatalogService _bookCatalogService;

    public HomeController(IBookCatalogService bookCatalogService)
    {
        _bookCatalogService = bookCatalogService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await GetDashboardAsync());
    }

    public async Task<IActionResult> RatingArena()
    {
        return View(await GetDashboardAsync());
    }

    private Task<BookDashboardViewModel> GetDashboardAsync()
    {
        return _bookCatalogService.GetDashboardAsync();
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
