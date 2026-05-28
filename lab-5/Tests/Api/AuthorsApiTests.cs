using System.Net;
using System.Net.Http.Json;
using Lab5.Dtos;
using Xunit;

namespace Lab5.Tests.Api;

public class AuthorsApiTests : IClassFixture<Lab5ApiFactory>
{
    private readonly HttpClient _client;

    public AuthorsApiTests(Lab5ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededAuthors()
    {
        var response = await _client.GetAsync("/api/authors");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var authors = await response.Content.ReadFromJsonAsync<List<AuthorDto>>();
        Assert.NotNull(authors);
        Assert.NotEmpty(authors!);
    }

    [Fact]
    public async Task GetById_Returns404_ForMissingAuthor()
    {
        var response = await _client.GetAsync("/api/authors/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_CreatesAuthor_AndReturnsCreated()
    {
        var request = new AuthorUpsertDto
        {
            FirstName = "Test",
            LastName = "Author",
            Biography = "Integration test author",
            BirthDate = new DateTime(1990, 1, 1),
            Nationality = "Croatian",
            Website = "https://example.com"
        };

        var response = await _client.PostAsJsonAsync("/api/authors", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(created);
        Assert.Equal("Test", created!.FirstName);
        Assert.Equal("Author", created.LastName);
    }
}