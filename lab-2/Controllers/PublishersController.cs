using Lab2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

public class PublishersController : Controller
{
    private readonly IPublisherRepository _publisherRepository;

    public PublishersController(IPublisherRepository publisherRepository)
    {
        _publisherRepository = publisherRepository;
    }

    public async Task<IActionResult> Index()
    {
        var publishers = await _publisherRepository.GetAllAsync();
        return View(publishers);
    }

    public async Task<IActionResult> Details(int id)
    {
        var publisher = await _publisherRepository.GetByIdAsync(id);
        if (publisher is null)
        {
            return NotFound();
        }

        return View(publisher);
    }
}
