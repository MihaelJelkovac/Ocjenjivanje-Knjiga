using Lab3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers;

[Route("[controller]")]
[Route("izdavaci")]
public class PublishersController : Controller
{
    private readonly IPublisherRepository _publisherRepository;

    public PublishersController(IPublisherRepository publisherRepository)
    {
        _publisherRepository = publisherRepository;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var publishers = await _publisherRepository.GetAllAsync();
        return View(publishers);
    }

    [Route("{id:int}")]
    [Route("detalji/{id:int}")]
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
