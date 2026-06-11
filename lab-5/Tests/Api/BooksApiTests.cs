using System.Net;
using System.Net.Http.Json;
using Lab5.Dtos;
using Lab5.Models;
using Xunit;

namespace Lab5.Tests.Api;

/// <summary>
/// Integracijski testovi za Books API
/// Testira sve CRUD operacije na /api/books
/// </summary>
public class BooksApiTests : IClassFixture<Lab5TestFactory>
{
    private readonly HttpClient _client;

    public BooksApiTests(Lab5TestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededBooks()
    {
        // Act
        var response = await _client.GetAsync("/api/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
        Assert.NotNull(books);
        Assert.NotEmpty(books);
    }

    [Fact]
    public async Task GetAll_WithQuery_FiltersResults()
    {
        // Act
        var response = await _client.GetAsync("/api/books?query=Test");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
        Assert.NotNull(books);
    }

    [Fact]
    public async Task GetById_ReturnsSeededBook_WithNestedDTOs()
    {
        // Act
        var response = await _client.GetAsync("/api/books/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var book = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(book);
        Assert.NotNull(book!.Author);
        Assert.NotNull(book.Publisher);
    }

    [Fact]
    public async Task GetById_WithInvalidId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/books/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_CreatesBook_AndReturnsCreated()
    {
        // Arrange
        var request = new BookUpsertDto
        {
            Title = "New Test Book",
            Isbn = "9876543210",
            Description = "A test book for integration tests",
            PublishedOn = new DateTime(2024, 1, 1),
            PageCount = 300,
            Language = "English",
            Status = BookStatus.Available,
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("New Test Book", created.Title);
        Assert.Equal("9876543210", created.Isbn);
    }

    [Fact]
    public async Task Post_ValidationFailure_ReturnsBadRequest()
    {
        // Arrange
        var invalid = new BookUpsertDto
        {
            Title = string.Empty,
            Isbn = string.Empty,
            Description = string.Empty,
            PublishedOn = DateTime.UtcNow,
            PageCount = 0,
            Language = string.Empty,
            Status = BookStatus.Available,
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalid);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidISBN_ReturnsBadRequest()
    {
        // Arrange - ISBN too short
        var invalid = new BookUpsertDto
        {
            Title = "Test Book",
            Isbn = "123", // Too short (min 10 chars)
            Description = "Description",
            PublishedOn = DateTime.UtcNow,
            PageCount = 100,
            Language = "English",
            Status = BookStatus.Available,
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalid);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidPageCount_ReturnsBadRequest()
    {
        // Arrange - PageCount must be > 0
        var invalid = new BookUpsertDto
        {
            Title = "Test Book",
            Isbn = "1234567890",
            Description = "Description",
            PublishedOn = DateTime.UtcNow,
            PageCount = -5, // Invalid: must be > 0
            Language = "English",
            Status = BookStatus.Available,
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalid);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesBook_AndReturnsOk()
    {
        // Arrange - Create a new book first
        var createRequest = new BookUpsertDto
        {
            Title = "Original Title",
            Isbn = "1111111111",
            Description = "Original description",
            PublishedOn = new DateTime(2024, 1, 1),
            PageCount = 200,
            Language = "English",
            Status = BookStatus.Available,
            AuthorId = 1,
            PublisherId = 1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/books", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<BookDto>();

        // Update the created book
        var update = new BookUpsertDto
        {
            Title = "Updated Title",
            Isbn = "2222222222",
            Description = "Updated description",
            PublishedOn = new DateTime(2024, 6, 1),
            PageCount = 250,
            Language = "Croatian",
            Status = BookStatus.Reserved,
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/books/{created!.Id}", update);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(updated);
        Assert.Equal("Updated Title", updated!.Title);
        Assert.Equal("2222222222", updated.Isbn);
        Assert.Equal(250, updated.PageCount);
    }

    [Fact]
    public async Task Put_WithNonExistentId_Returns404()
    {
        // Arrange
        var update = new BookUpsertDto
        {
            Title = "Test",
            Isbn = "1234567890",
            Description = "Test",
            PublishedOn = DateTime.UtcNow,
            PageCount = 100,
            Language = "English",
            Status = BookStatus.Available,
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/books/99999", update);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesBook_AndReturnsNoContent()
    {
        // Arrange - Create a new book first
        var createRequest = new BookUpsertDto
        {
            Title = "Book to Delete",
            Isbn = "9999999999",
            Description = "This book will be deleted",
            PublishedOn = new DateTime(2024, 1, 1),
            PageCount = 100,
            Language = "English",
            Status = BookStatus.Available,
            AuthorId = 1,
            PublisherId = 1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/books", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<BookDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/books/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/books/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/books/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}