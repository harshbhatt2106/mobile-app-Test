using System.Net.Http.Json;
using MobileAppTest.Models;

namespace MobileAppTest.Services;

public sealed class UserService
{
    private const string BaseUrl = "http://192.168.31.212:8080/api/Users";
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<User>>(BaseUrl, cancellationToken)
               ?? new List<User>();
    }

    public async Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        // Id is intentionally not sent for create; SQL Server generates it.
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, new
        {
            user.Name,
            user.Email,
            user.Age
        }, cancellationToken);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<User>(cancellationToken: cancellationToken);
    }

    public async Task<User?> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{user.Id}", new
        {
            user.Name,
            user.Email,
            user.Age
        }, cancellationToken);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<User>(cancellationToken: cancellationToken);
    }

    public async Task DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
