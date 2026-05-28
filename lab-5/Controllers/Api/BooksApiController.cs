using Lab5.Dtos;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers.Api;

[ApiController]
[Route("api/books")]
public class BooksApiController : ControllerBase
{
    private readonly IBookRepository _bookRepository;

    public BooksApiController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll([FromQuery] string? query = null)
    {
        var books = await _bookRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = query.Trim();
            books = books.Where(book =>
                book.Title.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                (book.Author != null && (
                    book.Author.FirstName.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                    book.Author.LastName.Contains(normalized, StringComparison.OrdinalIgnoreCase))) ||
                (book.Publisher != null && book.Publisher.Name.Contains(normalized, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        return Ok(books.Select(ApiDtoMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDto>> GetById(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book is null ? NotFound() : Ok(ApiDtoMapper.ToDto(book));
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create([FromBody] BookUpsertDto model)
    {
        var book = await _bookRepository.CreateAsync(new Book
        {
            Title = model.Title,
            Isbn = model.Isbn,
            Description = model.Description,
            PublishedOn = model.PublishedOn,
            PageCount = model.PageCount,
            Language = model.Language,
            Status = model.Status,
            AuthorId = model.AuthorId,
            PublisherId = model.PublisherId
        });

        var created = await _bookRepository.GetByIdAsync(book.Id);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, ApiDtoMapper.ToDto(created ?? book));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookDto>> Update(int id, [FromBody] BookUpsertDto model)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        book.Title = model.Title;
        book.Isbn = model.Isbn;
        book.Description = model.Description;
        book.PublishedOn = model.PublishedOn;
        book.PageCount = model.PageCount;
        book.Language = model.Language;
        book.Status = model.Status;
        book.AuthorId = model.AuthorId;
        book.PublisherId = model.PublisherId;

        await _bookRepository.UpdateAsync(book);
        var updated = await _bookRepository.GetByIdAsync(book.Id);
        return Ok(ApiDtoMapper.ToDto(updated ?? book));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _bookRepository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}