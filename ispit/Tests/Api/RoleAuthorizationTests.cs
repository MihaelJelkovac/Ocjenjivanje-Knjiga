using System.Net;
using System.Net.Http.Json;
using Lab5.Dtos;
using Lab5.Models;
using Xunit;

namespace Lab5.Tests.Api;

public class RoleAuthorizationTests : IClassFixture<Lab5TestFactory>
{
    private readonly HttpClient _client;

    public RoleAuthorizationTests(Lab5TestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Manager_CanCreate_But_CannotDelete_AdminOnly()
    {
        // Arrange - create a book as Admin (default)
        var createRequest = new BookUpsertDto
        {
            Title = "Role Test Book",
            Isbn = "5555555555",
            Description = "Created for role tests",
            PublishedOn = DateTime.UtcNow,
            PageCount = 150,
            Language = "English",
            Status = BookStatus.Available,
            AuthorId = 1,
            PublisherId = 1
        };

        var createResponse = await _client.PostAsJsonAsync("/api/books", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(created);

        // Act - attempt to create as Manager (should be allowed by AuthorizeAdminManager)
        var msg = new HttpRequestMessage(HttpMethod.Post, "/api/books");
        msg.Headers.Add("X-Auth-Role", "Manager");
        msg.Content = JsonContent.Create(createRequest);
        var managerCreate = await _client.SendAsync(msg);

        // Assert - Manager should be allowed to create
        Assert.Equal(HttpStatusCode.Created, managerCreate.StatusCode);

        // Act - attempt to delete the originally created book as Manager (Delete is Admin-only)
        var del = new HttpRequestMessage(HttpMethod.Delete, $"/api/books/{created!.Id}");
        del.Headers.Add("X-Auth-Role", "Manager");
        var managerDelete = await _client.SendAsync(del);

        // Assert - Manager should NOT be allowed to delete (403 Forbidden)
        Assert.Equal(HttpStatusCode.Forbidden, managerDelete.StatusCode);
    }

    [Fact]
    public async Task Admin_CanDelete_AsExpected()
    {
        // Arrange - create a book as Admin
        var createRequest = new BookUpsertDto
        {
            Title = "Admin Delete Book",
            Isbn = "6666666666",
            Description = "Created to test admin delete",
            PublishedOn = DateTime.UtcNow,
            PageCount = 120,
            Language = "English",
            Status = BookStatus.Available,
            AuthorId = 1,
            PublisherId = 1
        };

        var createResponse = await _client.PostAsJsonAsync("/api/books", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(created);

        // Act - delete as Admin (default)
        var deleteResponse = await _client.DeleteAsync($"/api/books/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}
