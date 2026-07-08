using System.Net;
using System.Net.Http.Json;
using Lab5.Dtos;
using Xunit;

namespace Lab5.Tests.Api;

public class BooksCrudTests : IClassFixture<Lab5TestFactory>
{
    private readonly HttpClient _client;

    public BooksCrudTests(Lab5TestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var resp = await _client.GetAsync("/api/books");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var list = await resp.Content.ReadFromJsonAsync<List<BookDto>>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Create_Update_Delete_Cycle_Works_WhenAuthorized()
    {
        // Create
        var create = new BookUpsertDto
        {
            Title = "Integration Test Book",
            Isbn = "999-0-12-345678-9",
            Description = "Created by integration test",
            PublishedOn = DateTime.UtcNow,
            PageCount = 123,
            Language = "EN",
            Status = Lab5.Models.BookStatus.Available,
            AuthorId = 1,
            PublisherId = 1
        };

        var postResp = await _client.PostAsJsonAsync("/api/books", create);
        Assert.Equal(HttpStatusCode.Created, postResp.StatusCode);

        var created = await postResp.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);

        // Update
        var update = new BookUpsertDto
        {
            Title = created.Title + " (updated)",
            Isbn = create.Isbn,
            Description = create.Description,
            PublishedOn = create.PublishedOn,
            PageCount = create.PageCount,
            Language = create.Language,
            Status = create.Status,
            AuthorId = create.AuthorId,
            PublisherId = create.PublisherId
        };

        var putResp = await _client.PutAsJsonAsync($"/api/books/{created.Id}", update);
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);

        var updated = await putResp.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(updated);
        Assert.Equal(update.Title, updated!.Title);

        // Delete
        var delResp = await _client.DeleteAsync($"/api/books/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);

        // Confirm deletion
        var getResp = await _client.GetAsync($"/api/books/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }
}
