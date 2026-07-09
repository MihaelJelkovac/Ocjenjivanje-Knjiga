using Lab5.Authorization;
using Lab5.Dtos;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers.Api;

[ApiController]
[Route("api/users")]
public class UsersApiController : BaseApiController
{
    private readonly IUserRepository _repository;

    public UsersApiController(IUserRepository repository, ILogger<UsersApiController> logger) : base(logger)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll([FromQuery] string? query = null)
    {
        var users = await _repository.GetAllAsync();
        var filtered = ApplyQueryFilter(users, query,
            u => new[] { u.Username, u.FullName, u.Email });
        return Ok(filtered.Select(ApiDtoMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(ApiDtoMapper.ToDto(user));
    }

    [HttpPost]
    [AuthorizeAdminManager]
    public async Task<ActionResult<UserDto>> Create([FromBody] UserUpsertDto model)
    {
        var user = await _repository.CreateAsync(new User
        {
            Username = model.Username,
            FullName = model.FullName,
            Email = model.Email,
            JoinedAt = model.JoinedAt,
            FavoriteGenre = model.FavoriteGenre,
            ReputationPoints = model.ReputationPoints,
            IsPremiumMember = model.IsPremiumMember
        });

        Logger.LogInformation("✅ [API] Korisnik kreiran: {UserId} - {Username}", user.Id, user.Username);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, ApiDtoMapper.ToDto(user));
    }

    [HttpPut("{id:int}")]
    [AuthorizeAdminManager]
    public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UserUpsertDto model)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user is null) return NotFound();

        user.Username = model.Username;
        user.FullName = model.FullName;
        user.Email = model.Email;
        user.JoinedAt = model.JoinedAt;
        user.FavoriteGenre = model.FavoriteGenre;
        user.ReputationPoints = model.ReputationPoints;
        user.IsPremiumMember = model.IsPremiumMember;

        await _repository.UpdateAsync(user);
        Logger.LogInformation("✅ [API] Korisnik ažuriran: {UserId}", user.Id);
        return Ok(ApiDtoMapper.ToDto(user));
    }

    [HttpDelete("{id:int}")]
    [AuthorizeAdmin]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        Logger.LogInformation(deleted ? "✅ [API] Korisnik obrisan: {UserId}" : "⚠️ [API] Korisnik nije pronađen: {UserId}", id);
        return deleted ? NoContent() : NotFound();
    }
}