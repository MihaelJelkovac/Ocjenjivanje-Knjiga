using Lab5.Authorization;
using Lab5.Dtos;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers.Api;

[ApiController]
[Route("api/books")]
public class BooksApiController : BaseApiController
{
    private readonly IBookRepository _bookRepository;

    public BooksApiController(IBookRepository bookRepository, ILogger<BooksApiController> logger) : base(logger)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll([FromQuery] string? query = null)
    {
        var books = await _bookRepository.GetAllAsync();

        // Filter books by title, author names, or publisher name
        var filtered = ApplyQueryFilter(books, query,
            b => new[] {
                b.Title,
                b.Author?.FirstName ?? "",
                b.Author?.LastName ?? "",
                b.Publisher?.Name ?? ""
            }.Where(s => !string.IsNullOrEmpty(s)).ToArray());

        return Ok(filtered.Select(ApiDtoMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDto>> GetById(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book is null ? NotFound() : Ok(ApiDtoMapper.ToDto(book));
    }

    [HttpPost]
    [AuthorizeAdminManager]
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

        Logger.LogInformation("✅ [API] Knjiga kreirana: {BookId} - {BookTitle}", book.Id, book.Title);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, ApiDtoMapper.ToDto(book));
    }

    [HttpPut("{id:int}")]
    [AuthorizeAdminManager]
    public async Task<ActionResult<BookDto>> Update(int id, [FromBody] BookUpsertDto model)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null) return NotFound();

        // Update only allowed fields
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
        Logger.LogInformation("✅ [API] Knjiga ažurirana: {BookId}", book.Id);
        return Ok(ApiDtoMapper.ToDto(book));
    }

    [HttpDelete("{id:int}")]
    [AuthorizeAdmin]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _bookRepository.DeleteAsync(id);
        Logger.LogInformation(deleted ? "✅ [API] Knjiga obrisana: {BookId}" : "⚠️ [API] Knjiga nije pronađena: {BookId}", id);
        return deleted ? NoContent() : NotFound();
    }
}