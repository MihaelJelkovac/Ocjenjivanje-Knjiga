using System.Net;
using System.Net.Http.Json;
using Lab5.Dtos;
using Xunit;

namespace Lab5.Tests.Api;

/// <summary>
/// Integracijski testovi za Genres API
/// Testira sve CRUD operacije na /api/genres
/// </summary>
public class GenresApiTests : IClassFixture<Lab5TestFactory>
{
    private readonly HttpClient _client;

    public GenresApiTests(Lab5TestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededGenres()
    {
        // Act
        var response = await _client.GetAsync("/api/genres");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var genres = await response.Content.ReadFromJsonAsync<List<GenreDto>>();
        Assert.NotNull(genres);
        Assert.NotEmpty(genres);
        Assert.True(genres.Count >= 3);
    }

    [Fact]
    public async Task GetAll_WithQuery_FiltersResults()
    {
        // Act
        var response = await _client.GetAsync("/api/genres?query=Fantasy");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var genres = await response.Content.ReadFromJsonAsync<List<GenreDto>>();
        Assert.NotNull(genres);
        Assert.NotEmpty(genres);
        Assert.All(genres, g => Assert.Contains("Fantasy", g.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsGenre()
    {
        // Act
        var response = await _client.GetAsync("/api/genres/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var genre = await response.Content.ReadFromJsonAsync<GenreDto>();
        Assert.NotNull(genre);
        Assert.Equal(1, genre!.Id);
        Assert.Equal("Fantasy", genre.Name);
    }

    [Fact]
    public async Task GetById_WithInvalidId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/genres/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_CreatesGenre_AndReturnsCreated()
    {
        // Arrange
        var request = new GenreUpsertDto
        {
            Name = "Mystery",
            Description = "Crime and detective stories",
            Audience = "Adults"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/genres", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<GenreDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("Mystery", created.Name);
        Assert.Equal("Crime and detective stories", created.Description);
    }

    [Fact]
    public async Task Post_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange - Name is required and must be at least 2 chars
        var request = new GenreUpsertDto
        {
            Name = string.Empty,
            Description = string.Empty,
            Audience = string.Empty
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/genres", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesGenre_AndReturnsOk()
    {
        // Arrange - Create a new genre first
        var createRequest = new GenreUpsertDto
        {
            Name = "Romance",
            Description = "Love stories",
            Audience = "Adults"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/genres", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<GenreDto>();

        // Update the created genre
        var update = new GenreUpsertDto
        {
            Name = "Romance (Updated)",
            Description = "Updated description",
            Audience = "Teens and Adults"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/genres/{created!.Id}", update);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<GenreDto>();
        Assert.NotNull(updated);
        Assert.Equal("Romance (Updated)", updated!.Name);
    }

    [Fact]
    public async Task Put_WithNonExistentId_Returns404()
    {
        // Arrange
        var update = new GenreUpsertDto
        {
            Name = "Test",
            Description = "Test",
            Audience = "Test"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/genres/99999", update);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesGenre_AndReturnsNoContent()
    {
        // Arrange - Create a new genre first
        var createRequest = new GenreUpsertDto
        {
            Name = "Horror",
            Description = "Scary stories",
            Audience = "Adults"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/genres", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<GenreDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/genres/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/genres/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/genres/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
