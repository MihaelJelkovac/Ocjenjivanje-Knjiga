using Lab3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers;

[Route("[controller]")]
[Route("knjige")]
public class BooksController : Controller
{
    private readonly IBookRepository _bookRepository;

    public BooksController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var books = await _bookRepository.GetAllAsync();
        return View(books);
    }

    [Route("{id:int}")]
    [Route("detalji/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        return View(book);
    }
}
