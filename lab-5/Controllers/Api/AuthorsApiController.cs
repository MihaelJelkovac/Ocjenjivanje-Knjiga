using Lab5.Dtos;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers.Api;

[ApiController]
[Route("api/authors")]
public class AuthorsApiController : ControllerBase
{
    private readonly IAuthorRepository _repository;

    public AuthorsApiController(IAuthorRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll([FromQuery] string? query = null)
    {
        var authors = await _repository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = query.Trim();
            authors = authors.Where(a =>
                a.FirstName.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                a.LastName.Contains(normalized, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Ok(authors.Select(ApiDtoMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorDto>> GetById(int id)
    {
        var author = await _repository.GetByIdAsync(id);
        return author is null ? NotFound() : Ok(ApiDtoMapper.ToDto(author));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
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

        return CreatedAtAction(nameof(GetById), new { id = author.Id }, ApiDtoMapper.ToDto(author));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<AuthorDto>> Update(int id, [FromBody] AuthorUpsertDto model)
    {
        var author = await _repository.GetByIdAsync(id);
        if (author is null)
        {
            return NotFound();
        }

        author.FirstName = model.FirstName;
        author.LastName = model.LastName;
        author.Biography = model.Biography;
        author.BirthDate = model.BirthDate;
        author.Nationality = model.Nationality;
        author.Website = model.Website;

        await _repository.UpdateAsync(author);
        return Ok(ApiDtoMapper.ToDto(author));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}