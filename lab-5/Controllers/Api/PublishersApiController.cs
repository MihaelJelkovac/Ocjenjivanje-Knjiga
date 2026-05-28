using Lab5.Dtos;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers.Api;

[ApiController]
[Route("api/publishers")]
public class PublishersApiController : ControllerBase
{
    private readonly IPublisherRepository _repository;

    public PublishersApiController(IPublisherRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PublisherDto>>> GetAll([FromQuery] string? query = null)
    {
        var publishers = string.IsNullOrWhiteSpace(query)
            ? await _repository.GetAllAsync()
            : await _repository.SearchAsync(query);

        return Ok(publishers.Select(ApiDtoMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PublisherDto>> GetById(int id)
    {
        var publisher = await _repository.GetByIdAsync(id);
        return publisher is null ? NotFound() : Ok(ApiDtoMapper.ToDto(publisher));
    }

    [HttpPost]
    public async Task<ActionResult<PublisherDto>> Create([FromBody] PublisherUpsertDto model)
    {
        var publisher = await _repository.CreateAsync(new Publisher
        {
            Name = model.Name,
            City = model.City,
            Country = model.Country,
            FoundedOn = model.FoundedOn,
            Website = model.Website,
            ContactEmail = model.ContactEmail
        });

        return CreatedAtAction(nameof(GetById), new { id = publisher.Id }, ApiDtoMapper.ToDto(publisher));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PublisherDto>> Update(int id, [FromBody] PublisherUpsertDto model)
    {
        var publisher = await _repository.GetByIdAsync(id);
        if (publisher is null)
        {
            return NotFound();
        }

        publisher.Name = model.Name;
        publisher.City = model.City;
        publisher.Country = model.Country;
        publisher.FoundedOn = model.FoundedOn;
        publisher.Website = model.Website;
        publisher.ContactEmail = model.ContactEmail;

        await _repository.UpdateAsync(publisher);
        return Ok(ApiDtoMapper.ToDto(publisher));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}