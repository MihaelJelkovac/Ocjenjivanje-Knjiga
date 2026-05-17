using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lab4.Models;
using Lab4.Services;
using Lab4.ViewModels;

namespace Lab4.Controllers;

[Route("[controller]")]
public class HomeController : Controller
{
    private readonly IBookCatalogService _bookCatalogService;

    public HomeController(IBookCatalogService bookCatalogService)
    {
        _bookCatalogService = bookCatalogService;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        return View(await _bookCatalogService.GetDashboardAsync());
    }

    [Route("arena")]
    [Route("rating-arena")]
    public async Task<IActionResult> RatingArena()
    {
        return View(await _bookCatalogService.GetDashboardAsync());
    }

    [Route("privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [Route("error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
