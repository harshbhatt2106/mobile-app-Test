using System.Net.Http.Json;
using MobileAppTest.Models;

namespace MobileAppTest.Services;

public sealed class UserService
{
    private const string UsersEndpoint = "http://192.168.31.212:8080/api/Users";
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _httpClient.GetFromJsonAsync<List<User>>(UsersEndpoint, cancellationToken);
        return users ?? [];
    }

    public async Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(UsersEndpoint, user, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<User>(cancellationToken: cancellationToken);
    }
}
