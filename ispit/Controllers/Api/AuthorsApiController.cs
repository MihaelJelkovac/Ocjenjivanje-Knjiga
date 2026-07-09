using Lab5.Authorization;
using Lab5.Dtos;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers.Api;

[ApiController]
[Route("api/authors")]
public class AuthorsApiController : BaseApiController
{
    private readonly IAuthorRepository _repository;

    public AuthorsApiController(IAuthorRepository repository, ILogger<AuthorsApiController> logger) : base(logger)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll([FromQuery] string? query = null)
    {
        var authors = await _repository.GetAllAsync();
        var filtered = ApplyQueryFilter(authors, query,
            a => new[] { a.FirstName, a.LastName });
        return Ok(filtered.Select(ApiDtoMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorDto>> GetById(int id)
    {
        var author = await _repository.GetByIdAsync(id);
        return author is null ? NotFound() : Ok(ApiDtoMapper.ToDto(author));
    }

    [HttpPost]
    [AuthorizeAdminManager]
    public async Task<ActionResult<AuthorDto>> Create([FromBody] AuthorUpsertDto model)
    {
        var author = await _repository.CreateAsync(new Author
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Biography = model.Biography,
            BirthDate = model.BirthDate,
            Nationality = model.Nationality,
            Website = model.Website
        });

        Logger.LogInformation("✅ [API] Autor kreiran: {AuthorId} - {FirstName} {LastName}", author.Id, author.FirstName, author.LastName);
        return CreatedAtAction(nameof(GetById), new { id = author.Id }, ApiDtoMapper.ToDto(author));
    }

    [HttpPut("{id:int}")]
    [AuthorizeAdminManager]
    public async Task<ActionResult<AuthorDto>> Update(int id, [FromBody] AuthorUpsertDto model)
    {
        var author = await _repository.GetByIdAsync(id);
        if (author is null) return NotFound();

        author.FirstName = model.FirstName;
        author.LastName = model.LastName;
        author.Biography = model.Biography;
        author.BirthDate = model.BirthDate;
        author.Nationality = model.Nationality;
        author.Website = model.Website;

        await _repository.UpdateAsync(author);
        Logger.LogInformation("✅ [API] Autor ažuriran: {AuthorId}", author.Id);
        return Ok(ApiDtoMapper.ToDto(author));
    }

    [HttpDelete("{id:int}")]
    [AuthorizeAdmin]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        Logger.LogInformation(deleted ? "✅ [API] Autor obrisan: {AuthorId}" : "⚠️ [API] Autor nije pronađen: {AuthorId}", id);
        return deleted ? NoContent() : NotFound();
    }
}