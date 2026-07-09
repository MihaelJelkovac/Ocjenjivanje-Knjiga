using Lab5.Authorization;
using Lab5.Dtos;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers.Api;

[ApiController]
[Route("api/genres")]
public class GenresApiController : BaseApiController
{
    private readonly IGenreRepository _repository;

    public GenresApiController(IGenreRepository repository, ILogger<GenresApiController> logger) : base(logger)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetAll([FromQuery] string? query = null)
    {
        var genres = await _repository.GetAllAsync();
        var filtered = ApplyQueryFilter(genres, query, g => new[] { g.Name });
        return Ok(filtered.Select(ApiDtoMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GenreDto>> GetById(int id)
    {
        var genre = await _repository.GetByIdAsync(id);
        return genre is null ? NotFound() : Ok(ApiDtoMapper.ToDto(genre));
    }

    [HttpPost]
    [AuthorizeAdminManager]
    public async Task<ActionResult<GenreDto>> Create([FromBody] GenreUpsertDto model)
    {
        var genre = await _repository.CreateAsync(new Genre
        {
            Name = model.Name,
            Description = model.Description,
            Audience = model.Audience
        });

        Logger.LogInformation("✅ [API] Žanr kreiran: {GenreId} - {GenreName}", genre.Id, genre.Name);
        return CreatedAtAction(nameof(GetById), new { id = genre.Id }, ApiDtoMapper.ToDto(genre));
    }

    [HttpPut("{id:int}")]
    [AuthorizeAdminManager]
    public async Task<ActionResult<GenreDto>> Update(int id, [FromBody] GenreUpsertDto model)
    {
        var genre = await _repository.GetByIdAsync(id);
        if (genre is null) return NotFound();

        genre.Name = model.Name;
        genre.Description = model.Description;
        genre.Audience = model.Audience;

        await _repository.UpdateAsync(genre);
        Logger.LogInformation("✅ [API] Žanr ažuriran: {GenreId}", genre.Id);
        return Ok(ApiDtoMapper.ToDto(genre));
    }

    [HttpDelete("{id:int}")]
    [AuthorizeAdmin]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        Logger.LogInformation(deleted ? "✅ [API] Žanr obrisan: {GenreId}" : "⚠️ [API] Žanr nije pronađen: {GenreId}", id);
        return deleted ? NoContent() : NotFound();
    }
}