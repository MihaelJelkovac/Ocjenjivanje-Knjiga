using Lab2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

public class BooksController : Controller
{
    private readonly IBookRepository _bookRepository;

    public BooksController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _bookRepository.GetAllAsync();
        return View(books);
    }

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
