using System.Net;
using System.Net.Http.Json;
using Lab5.Dtos;
using Xunit;

namespace Lab5.Tests.Api;

/// <summary>
/// Integracijski testovi za Users API
/// Testira sve CRUD operacije na /api/users
/// </summary>
public class UsersApiTests : IClassFixture<Lab5TestFactory>
{
    private readonly HttpClient _client;

    public UsersApiTests(Lab5TestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededUsers()
    {
        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        Assert.NotNull(users);
        Assert.NotEmpty(users);
        Assert.True(users.Count >= 3);
    }

    [Fact]
    public async Task GetAll_WithQuery_FiltersResults()
    {
        // Act
        var response = await _client.GetAsync("/api/users?query=Alice");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        Assert.NotNull(users);
        Assert.NotEmpty(users);
        Assert.All(users, u =>
            Assert.True(
                u.Username.Contains("Alice", StringComparison.OrdinalIgnoreCase) ||
                u.FullName.Contains("Alice", StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains("Alice", StringComparison.OrdinalIgnoreCase)
            )
        );
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsUser()
    {
        // Act
        var response = await _client.GetAsync("/api/users/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal(1, user!.Id);
        Assert.Equal("reader1", user.Username);
        Assert.Equal("Alice Reader", user.FullName);
    }

    [Fact]
    public async Task GetById_WithInvalidId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/users/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_CreatesUser_AndReturnsCreated()
    {
        // Arrange
        var request = new UserUpsertDto
        {
            Username = "newreader",
            FullName = "David Johnson",
            Email = "david@example.com",
            JoinedAt = DateTime.UtcNow,
            FavoriteGenre = "Mystery",
            ReputationPoints = 0,
            IsPremiumMember = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("newreader", created.Username);
        Assert.Equal("David Johnson", created.FullName);
    }

    [Fact]
    public async Task Post_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange - Email must be valid
        var request = new UserUpsertDto
        {
            Username = "test",
            FullName = "Test User",
            Email = "not-an-email", // Invalid email
            JoinedAt = DateTime.UtcNow,
            FavoriteGenre = "Fiction",
            ReputationPoints = 0,
            IsPremiumMember = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithMissingRequiredFields_ReturnsBadRequest()
    {
        // Arrange - Username is required
        var request = new UserUpsertDto
        {
            Username = string.Empty, // Required
            FullName = "Test User",
            Email = "test@test.com",
            JoinedAt = DateTime.UtcNow,
            FavoriteGenre = "Fiction",
            ReputationPoints = 0,
            IsPremiumMember = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithUsernameTooShort_ReturnsBadRequest()
    {
        // Arrange - Username must be at least 3 characters
        var request = new UserUpsertDto
        {
            Username = "ab", // Too short (min 3 chars)
            FullName = "Test User",
            Email = "test@test.com",
            JoinedAt = DateTime.UtcNow,
            FavoriteGenre = "Fiction",
            ReputationPoints = 0,
            IsPremiumMember = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithNegativeReputationPoints_ReturnsBadRequest()
    {
        // Arrange - ReputationPoints must be >= 0
        var request = new UserUpsertDto
        {
            Username = "testuser",
            FullName = "Test User",
            Email = "test@test.com",
            JoinedAt = DateTime.UtcNow,
            FavoriteGenre = "Fiction",
            ReputationPoints = -10, // Invalid: must be >= 0
            IsPremiumMember = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesUser_AndReturnsOk()
    {
        // Arrange - Create a new user first
        var createRequest = new UserUpsertDto
        {
            Username = "newuser",
            FullName = "New User",
            Email = "newuser@example.com",
            JoinedAt = DateTime.UtcNow,
            FavoriteGenre = "Fantasy",
            ReputationPoints = 50,
            IsPremiumMember = false
        };
        var createResponse = await _client.PostAsJsonAsync("/api/users", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        // Update the created user
        var update = new UserUpsertDto
        {
            Username = "newuser_updated",
            FullName = "New User Updated",
            Email = "newuser.updated@example.com",
            JoinedAt = new DateTime(2023, 1, 15, 8, 30, 0),
            FavoriteGenre = "Science Fiction",
            ReputationPoints = 200,
            IsPremiumMember = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/users/{created!.Id}", update);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(updated);
        Assert.Equal("newuser_updated", updated!.Username);
        Assert.Equal("New User Updated", updated.FullName);
        Assert.Equal(200, updated.ReputationPoints);
    }

    [Fact]
    public async Task Put_WithNonExistentId_Returns404()
    {
        // Arrange
        var update = new UserUpsertDto
        {
            Username = "test",
            FullName = "Test",
            Email = "test@test.com",
            JoinedAt = DateTime.UtcNow,
            FavoriteGenre = "Fiction",
            ReputationPoints = 0,
            IsPremiumMember = false
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/99999", update);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesUser_AndReturnsNoContent()
    {
        // Arrange - Create a new user first
        var createRequest = new UserUpsertDto
        {
            Username = "tempuser",
            FullName = "Temporary User",
            Email = "temp@example.com",
            JoinedAt = DateTime.UtcNow,
            FavoriteGenre = "Poetry",
            ReputationPoints = 50,
            IsPremiumMember = false
        };
        var createResponse = await _client.PostAsJsonAsync("/api/users", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/users/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/users/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/users/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
