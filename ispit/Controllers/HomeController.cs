using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lab5.Models;
using Lab5.Services;
using Lab5.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Lab5.Controllers;

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

    [AllowAnonymous]
    [Route("health")]
    [Route("/Health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}

