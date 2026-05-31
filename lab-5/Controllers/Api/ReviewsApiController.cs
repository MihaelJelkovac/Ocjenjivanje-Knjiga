using Lab5.Dtos;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers.Api;

[ApiController]
[Route("api/reviews")]
public class ReviewsApiController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewsApiController(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAll([FromQuery] string? query = null)
    {
        var reviews = await _reviewRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = query.Trim();
            reviews = reviews.Where(review =>
                review.Title.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                review.Comment.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                (review.Book != null && review.Book.Title.Contains(normalized, StringComparison.OrdinalIgnoreCase)) ||
                (review.User != null && review.User.FullName.Contains(normalized, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        return Ok(reviews.Select(ApiDtoMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReviewDto>> GetById(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        return review is null ? NotFound() : Ok(ApiDtoMapper.ToDto(review));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
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

        var created = await _reviewRepository.GetByIdAsync(review.Id);
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, ApiDtoMapper.ToDto(created ?? review));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ReviewDto>> Update(int id, [FromBody] ReviewUpsertDto model)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review is null)
        {
            return NotFound();
        }

        review.Score = model.Score;
        review.Title = model.Title;
        review.Comment = model.Comment;
        review.ReviewedAt = model.ReviewedAt;
        review.IsRecommended = model.IsRecommended;
        review.Sentiment = model.Sentiment;
        review.BookId = model.BookId;
        review.UserId = model.UserId;

        await _reviewRepository.UpdateAsync(review);
        var updated = await _reviewRepository.GetByIdAsync(review.Id);
        return Ok(ApiDtoMapper.ToDto(updated ?? review));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _reviewRepository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}