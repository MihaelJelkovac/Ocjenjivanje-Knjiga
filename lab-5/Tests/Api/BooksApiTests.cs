using System.Net;
using System.Net.Http.Json;
using Lab5.Dtos;
using Lab5.Models;
using Xunit;

namespace Lab5.Tests.Api;

public class BooksApiTests : IClassFixture<Lab5ApiFactory>
{
    private readonly HttpClient _client;

    public BooksApiTests(Lab5ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetById_ReturnsSeededBook_WithNestedDTOs()
    {
        var response = await _client.GetAsync("/api/books/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var book = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(book);
        Assert.NotNull(book!.Author);
        Assert.NotNull(book.Publisher);
    }

    [Fact]
    public async Task Post_ValidationFailure_ReturnsBadRequest()
    {
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

        var response = await _client.PostAsJsonAsync("/api/books", invalid);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}