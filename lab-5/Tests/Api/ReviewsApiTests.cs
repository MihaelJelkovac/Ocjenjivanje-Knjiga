using System.Net;
using System.Net.Http.Json;
using Lab5.Dtos;
using Lab5.Models;
using Xunit;

namespace Lab5.Tests.Api;

/// <summary>
/// Integracijski testovi za Reviews API
/// Testira sve CRUD operacije na /api/reviews
/// </summary>
public class ReviewsApiTests : IClassFixture<Lab5TestFactory>
{
    private readonly HttpClient _client;

    public ReviewsApiTests(Lab5TestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededReviews()
    {
        // Act
        var response = await _client.GetAsync("/api/reviews");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var reviews = await response.Content.ReadFromJsonAsync<List<ReviewDto>>();
        Assert.NotNull(reviews);
        Assert.NotEmpty(reviews);
        Assert.True(reviews.Count >= 3);
    }

    [Fact]
    public async Task GetAll_WithQuery_FiltersResults()
    {
        // Act
        var response = await _client.GetAsync("/api/reviews?query=Amazing");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var reviews = await response.Content.ReadFromJsonAsync<List<ReviewDto>>();
        Assert.NotNull(reviews);
        Assert.NotEmpty(reviews);
        Assert.All(reviews, r =>
            Assert.True(
                r.Title.Contains("Amazing", StringComparison.OrdinalIgnoreCase) ||
                r.Comment.Contains("Amazing", StringComparison.OrdinalIgnoreCase)
            )
        );
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsReviewWithNestedData()
    {
        // Act
        var response = await _client.GetAsync("/api/reviews/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var review = await response.Content.ReadFromJsonAsync<ReviewDto>();
        Assert.NotNull(review);
        Assert.Equal(1, review!.Id);
        Assert.Equal(5, review.Score);
        Assert.NotNull(review.Book);
        Assert.NotNull(review.User);
    }

    [Fact]
    public async Task GetById_WithInvalidId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/reviews/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_CreatesReview_AndReturnsCreated()
    {
        // Arrange
        var request = new ReviewUpsertDto
        {
            Score = 4,
            Title = "Very Good",
            Comment = "Enjoyed reading this book",
            ReviewedAt = DateTime.UtcNow,
            IsRecommended = true,
            Sentiment = ReviewSentiment.Positive,
            BookId = 1,
            UserId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<ReviewDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal(4, created.Score);
        Assert.Equal("Very Good", created.Title);
    }

    [Fact]
    public async Task Post_WithScoreOutOfRange_ReturnsBadRequest()
    {
        // Arrange - Score must be between 1 and 5
        var request = new ReviewUpsertDto
        {
            Score = 10, // Invalid: out of range
            Title = "Test",
            Comment = "Test",
            ReviewedAt = DateTime.UtcNow,
            IsRecommended = true,
            Sentiment = ReviewSentiment.Positive,
            BookId = 1,
            UserId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithEmptyTitle_ReturnsBadRequest()
    {
        // Arrange - Title is required
        var request = new ReviewUpsertDto
        {
            Score = 5,
            Title = string.Empty, // Invalid: required
            Comment = "Test comment",
            ReviewedAt = DateTime.UtcNow,
            IsRecommended = true,
            Sentiment = ReviewSentiment.Enthusiastic,
            BookId = 1,
            UserId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithTitleTooShort_ReturnsBadRequest()
    {
        // Arrange - Title must be at least 3 characters
        var request = new ReviewUpsertDto
        {
            Score = 5,
            Title = "Ok", // Too short (min 3 chars)
            Comment = "Test",
            ReviewedAt = DateTime.UtcNow,
            IsRecommended = true,
            Sentiment = ReviewSentiment.Positive,
            BookId = 1,
            UserId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesReview_AndReturnsOk()
    {
        // Arrange - Create a new review first
        var createRequest = new ReviewUpsertDto
        {
            Score = 4,
            Title = "Initial Review",
            Comment = "Initial comment",
            ReviewedAt = DateTime.UtcNow,
            IsRecommended = true,
            Sentiment = ReviewSentiment.Positive,
            BookId = 1,
            UserId = 1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/reviews", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ReviewDto>();

        // Update the created review
        var update = new ReviewUpsertDto
        {
            Score = 3,
            Title = "Good but not great",
            Comment = "It was okay",
            ReviewedAt = DateTime.UtcNow,
            IsRecommended = false,
            Sentiment = ReviewSentiment.Neutral,
            BookId = 1,
            UserId = 1
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/reviews/{created!.Id}", update);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<ReviewDto>();
        Assert.NotNull(updated);
        Assert.Equal(3, updated!.Score);
        Assert.Equal("Good but not great", updated.Title);
    }

    [Fact]
    public async Task Put_WithNonExistentId_Returns404()
    {
        // Arrange
        var update = new ReviewUpsertDto
        {
            Score = 5,
            Title = "Test",
            Comment = "Test",
            ReviewedAt = DateTime.UtcNow,
            IsRecommended = true,
            Sentiment = ReviewSentiment.Positive,
            BookId = 1,
            UserId = 1
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/reviews/99999", update);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesReview_AndReturnsNoContent()
    {
        // Arrange - Create a new review first
        var createRequest = new ReviewUpsertDto
        {
            Score = 5,
            Title = "Excellent Book",
            Comment = "One of the best",
            ReviewedAt = DateTime.UtcNow,
            IsRecommended = true,
            Sentiment = ReviewSentiment.Enthusiastic,
            BookId = 1,
            UserId = 1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/reviews", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ReviewDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/reviews/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/reviews/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/reviews/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
