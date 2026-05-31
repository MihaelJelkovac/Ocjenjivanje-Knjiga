using Lab5.Dtos;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers.Api;

[ApiController]
[Route("api/genres")]
public class GenresApiController : ControllerBase
{
    private readonly IGenreRepository _repository;

    public GenresApiController(IGenreRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetAll([FromQuery] string? query = null)
    {
        var genres = await _repository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = query.Trim();
            genres = genres.Where(g =>
                g.Name.Contains(normalized, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Ok(genres.Select(ApiDtoMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GenreDto>> GetById(int id)
    {
        var genre = await _repository.GetByIdAsync(id);
        return genre is null ? NotFound() : Ok(ApiDtoMapper.ToDto(genre));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<GenreDto>> Create([FromBody] GenreUpsertDto model)
    {
        var genre = await _repository.CreateAsync(new Genre
        {
            Name = model.Name,
            Description = model.Description,
            Audience = model.Audience
        });

        return CreatedAtAction(nameof(GetById), new { id = genre.Id }, ApiDtoMapper.ToDto(genre));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<GenreDto>> Update(int id, [FromBody] GenreUpsertDto model)
    {
        var genre = await _repository.GetByIdAsync(id);
        if (genre is null)
        {
            return NotFound();
        }

        genre.Name = model.Name;
        genre.Description = model.Description;
        genre.Audience = model.Audience;

        await _repository.UpdateAsync(genre);
        return Ok(ApiDtoMapper.ToDto(genre));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}