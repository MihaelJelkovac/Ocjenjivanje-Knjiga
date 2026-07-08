using System.Net;
using System.Net.Http.Json;
using Lab5.Dtos;
using Xunit;

namespace Lab5.Tests.Api;

/// <summary>
/// Integracijski testovi za Publishers API
/// Testira sve CRUD operacije na /api/publishers
/// </summary>
public class PublishersApiTests : IClassFixture<Lab5TestFactory>
{
    private readonly HttpClient _client;

    public PublishersApiTests(Lab5TestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededPublishers()
    {
        // Act
        var response = await _client.GetAsync("/api/publishers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var publishers = await response.Content.ReadFromJsonAsync<List<PublisherDto>>();
        Assert.NotNull(publishers);
        Assert.NotEmpty(publishers);
        Assert.True(publishers.Count >= 3);
    }

    [Fact]
    public async Task GetAll_WithQuery_FiltersResults()
    {
        // Act
        var response = await _client.GetAsync("/api/publishers?query=Bloomsbury");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var publishers = await response.Content.ReadFromJsonAsync<List<PublisherDto>>();
        Assert.NotNull(publishers);
        Assert.NotEmpty(publishers);
        Assert.All(publishers, p => Assert.Contains("Bloomsbury", p.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsPublisher()
    {
        // Act
        var response = await _client.GetAsync("/api/publishers/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var publisher = await response.Content.ReadFromJsonAsync<PublisherDto>();
        Assert.NotNull(publisher);
        Assert.Equal(1, publisher!.Id);
        Assert.Equal("Bloomsbury", publisher.Name);
        Assert.Equal("London", publisher.City);
    }

    [Fact]
    public async Task GetById_WithInvalidId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/publishers/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_CreatesPublisher_AndReturnsCreated()
    {
        // Arrange
        var request = new PublisherUpsertDto
        {
            Name = "Penguin Books",
            City = "London",
            Country = "UK",
            FoundedOn = new DateTime(1935, 1, 1),
            Website = "https://www.penguin.co.uk",
            ContactEmail = "contact@penguin.co.uk"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/publishers", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<PublisherDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("Penguin Books", created.Name);
        Assert.Equal("London", created.City);
    }

    [Fact]
    public async Task Post_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange - Name and City are required
        var request = new PublisherUpsertDto
        {
            Name = string.Empty,
            City = string.Empty,
            Country = string.Empty,
            FoundedOn = DateTime.UtcNow,
            Website = "invalid-url",
            ContactEmail = "invalid-email"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/publishers", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new PublisherUpsertDto
        {
            Name = "Test Publisher",
            City = "Test City",
            Country = "Test Country",
            FoundedOn = new DateTime(2024, 1, 1),
            Website = "https://test.com",
            ContactEmail = "not-an-email" // Invalid email
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/publishers", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesPublisher_AndReturnsOk()
    {
        // Arrange - Create a new publisher first
        var createRequest = new PublisherUpsertDto
        {
            Name = "Test Publisher",
            City = "Test City",
            Country = "Test Country",
            FoundedOn = new DateTime(2024, 1, 1),
            Website = "https://test.com",
            ContactEmail = "test@test.com"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/publishers", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PublisherDto>();

        // Update the created publisher
        var update = new PublisherUpsertDto
        {
            Name = "Test Publisher (Updated)",
            City = "Updated City",
            Country = "Updated Country",
            FoundedOn = new DateTime(2024, 1, 1),
            Website = "https://test-updated.com",
            ContactEmail = "updated@test.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/publishers/{created!.Id}", update);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<PublisherDto>();
        Assert.NotNull(updated);
        Assert.Equal("Test Publisher (Updated)", updated!.Name);
    }

    [Fact]
    public async Task Put_WithNonExistentId_Returns404()
    {
        // Arrange
        var update = new PublisherUpsertDto
        {
            Name = "Test",
            City = "Test",
            Country = "Test",
            FoundedOn = DateTime.UtcNow,
            Website = "https://test.com",
            ContactEmail = "test@test.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/publishers/99999", update);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesPublisher_AndReturnsNoContent()
    {
        // Arrange - Create a new publisher first
        var createRequest = new PublisherUpsertDto
        {
            Name = "Test Publisher",
            City = "Test City",
            Country = "Test Country",
            FoundedOn = new DateTime(2024, 1, 1),
            Website = "https://test.com",
            ContactEmail = "test@test.com"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/publishers", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PublisherDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/publishers/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/publishers/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/publishers/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
