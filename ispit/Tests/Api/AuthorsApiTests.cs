using System.Net;
using System.Net.Http.Json;
using Lab5.Dtos;
using Xunit;

namespace Lab5.Tests.Api;

/// <summary>
/// Integracijski testovi za Authors API
/// Testira sve CRUD operacije na /api/authors
/// </summary>
public class AuthorsApiTests : IClassFixture<Lab5TestFactory>
{
    private readonly HttpClient _client;

    public AuthorsApiTests(Lab5TestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededAuthors()
    {
        // Act
        var response = await _client.GetAsync("/api/authors");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var authors = await response.Content.ReadFromJsonAsync<List<AuthorDto>>();
        Assert.NotNull(authors);
        Assert.NotEmpty(authors!);
    }

    [Fact]
    public async Task GetAll_WithQuery_FiltersResults()
    {
        // Act
        var response = await _client.GetAsync("/api/authors?query=Test");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var authors = await response.Content.ReadFromJsonAsync<List<AuthorDto>>();
        Assert.NotNull(authors);
    }

    [Fact]
    public async Task GetById_ReturnsAuthor_WithValidId()
    {
        // Act
        var response = await _client.GetAsync("/api/authors/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var author = await response.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);
        Assert.Equal(1, author!.Id);
    }

    [Fact]
    public async Task GetById_Returns404_ForMissingAuthor()
    {
        // Act
        var response = await _client.GetAsync("/api/authors/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_CreatesAuthor_AndReturnsCreated()
    {
        // Arrange
        var request = new AuthorUpsertDto
        {
            FirstName = "Test",
            LastName = "Author",
            Biography = "Integration test author",
            BirthDate = new DateTime(1990, 1, 1),
            Nationality = "Croatian",
            Website = "https://example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/authors", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("Test", created.FirstName);
        Assert.Equal("Author", created.LastName);
    }

    [Fact]
    public async Task Post_WithMissingRequiredFields_ReturnsBadRequest()
    {
        // Arrange - FirstName and LastName are required
        var request = new AuthorUpsertDto
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Biography = "Test",
            BirthDate = DateTime.UtcNow,
            Nationality = "Test",
            Website = "https://test.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/authors", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithNameTooShort_ReturnsBadRequest()
    {
        // Arrange - Name must be at least 2 characters
        var request = new AuthorUpsertDto
        {
            FirstName = "A", // Too short (min 2 chars)
            LastName = "A",  // Too short (min 2 chars)
            Biography = "Test",
            BirthDate = DateTime.UtcNow,
            Nationality = "Test",
            Website = "https://test.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/authors", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidWebsite_ReturnsBadRequest()
    {
        // Arrange - Website must be a valid URL
        var request = new AuthorUpsertDto
        {
            FirstName = "Test",
            LastName = "Author",
            Biography = "Test",
            BirthDate = DateTime.UtcNow,
            Nationality = "Test",
            Website = "not-a-valid-url" // Invalid URL
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/authors", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesAuthor_AndReturnsOk()
    {
        // Arrange - Create a new author first
        var createRequest = new AuthorUpsertDto
        {
            FirstName = "John",
            LastName = "Original",
            Biography = "Original biography",
            BirthDate = new DateTime(1980, 1, 1),
            Nationality = "British",
            Website = "https://original.com"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/authors", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<AuthorDto>();

        // Update the created author
        var update = new AuthorUpsertDto
        {
            FirstName = "Jonathan",
            LastName = "Updated",
            Biography = "Updated biography",
            BirthDate = new DateTime(1985, 6, 15),
            Nationality = "American",
            Website = "https://updated.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/authors/{created!.Id}", update);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(updated);
        Assert.Equal("Jonathan", updated!.FirstName);
        Assert.Equal("Updated", updated.LastName);
        Assert.Equal("Updated biography", updated.Biography);
    }

    [Fact]
    public async Task Put_WithNonExistentId_Returns404()
    {
        // Arrange
        var update = new AuthorUpsertDto
        {
            FirstName = "Test",
            LastName = "Author",
            Biography = "Test",
            BirthDate = DateTime.UtcNow,
            Nationality = "Test",
            Website = "https://test.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/authors/99999", update);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesAuthor_AndReturnsNoContent()
    {
        // Arrange - Create a new author first
        var createRequest = new AuthorUpsertDto
        {
            FirstName = "Deletable",
            LastName = "Author",
            Biography = "Will be deleted",
            BirthDate = new DateTime(1995, 5, 10),
            Nationality = "Croatian",
            Website = "https://delete.com"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/authors", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<AuthorDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/authors/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/authors/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/authors/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}