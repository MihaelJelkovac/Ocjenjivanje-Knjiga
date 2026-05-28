using Lab5.Dtos;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers.Api;

[ApiController]
[Route("api/users")]
public class UsersApiController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UsersApiController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll([FromQuery] string? query = null)
    {
        var users = await _repository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = query.Trim();
            users = users.Where(u =>
                u.Username.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                u.FullName.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(normalized, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Ok(users.Select(ApiDtoMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(ApiDtoMapper.ToDto(user));
    }

    [HttpPost]
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

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, ApiDtoMapper.ToDto(user));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UserUpsertDto model)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        user.Username = model.Username;
        user.FullName = model.FullName;
        user.Email = model.Email;
        user.JoinedAt = model.JoinedAt;
        user.FavoriteGenre = model.FavoriteGenre;
        user.ReputationPoints = model.ReputationPoints;
        user.IsPremiumMember = model.IsPremiumMember;

        await _repository.UpdateAsync(user);
        return Ok(ApiDtoMapper.ToDto(user));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}