using Lab5.Authorization;
using Lab5.Dtos;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers.Api;

[ApiController]
[Route("api/reviews")]
public class ReviewsApiController : BaseApiController
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewsApiController(IReviewRepository reviewRepository, ILogger<ReviewsApiController> logger) : base(logger)
    {
        _reviewRepository = reviewRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAll([FromQuery] string? query = null)
    {
        var reviews = await _reviewRepository.GetAllAsync();
        var filtered = ApplyQueryFilter(reviews, query,
            r => new[] {
                r.Title,
                r.Comment,
                r.Book?.Title ?? "",
                r.User?.FullName ?? ""
            }.Where(s => !string.IsNullOrEmpty(s)).ToArray());
        return Ok(filtered.Select(ApiDtoMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReviewDto>> GetById(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        return review is null ? NotFound() : Ok(ApiDtoMapper.ToDto(review));
    }

    [HttpPost]
    [AuthorizeAdminManager]
    public async Task<ActionResult<ReviewDto>> Create([FromBody] ReviewUpsertDto model)
    {
        var review = await _reviewRepository.CreateAsync(new Review
        {
            Score = model.Score,
            Title = model.Title,
            Comment = model.Comment,
            ReviewedAt = model.ReviewedAt,
            IsRecommended = model.IsRecommended,
            Sentiment = model.Sentiment,
            BookId = model.BookId,
            UserId = model.UserId
        });

        Logger.LogInformation("✅ [API] Recenzija kreirana: {ReviewId} za knjigu {BookId}", review.Id, review.BookId);
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, ApiDtoMapper.ToDto(review));
    }

    [HttpPut("{id:int}")]
    [AuthorizeAdminManager]
    public async Task<ActionResult<ReviewDto>> Update(int id, [FromBody] ReviewUpsertDto model)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review is null) return NotFound();

        review.Score = model.Score;
        review.Title = model.Title;
        review.Comment = model.Comment;
        review.ReviewedAt = model.ReviewedAt;
        review.IsRecommended = model.IsRecommended;
        review.Sentiment = model.Sentiment;
        review.BookId = model.BookId;
        review.UserId = model.UserId;

        await _reviewRepository.UpdateAsync(review);
        Logger.LogInformation("✅ [API] Recenzija ažurirana: {ReviewId}", review.Id);
        return Ok(ApiDtoMapper.ToDto(review));
    }

    [HttpDelete("{id:int}")]
    [AuthorizeAdmin]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _reviewRepository.DeleteAsync(id);
        Logger.LogInformation(deleted ? "✅ [API] Recenzija obrisana: {ReviewId}" : "⚠️ [API] Recenzija nije pronađena: {ReviewId}", id);
        return deleted ? NoContent() : NotFound();
    }
}