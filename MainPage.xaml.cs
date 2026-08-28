using MobileAppTest.Models;
using MobileAppTest.Services;

namespace MobileAppTest;

public partial class MainPage : ContentPage
{
    private readonly UserService _userService;
    private bool _loaded;

    public MainPage(UserService userService)
    {
        InitializeComponent();
        _userService = userService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_loaded)
        {
            _loaded = true;
            await LoadUsersAsync();
        }
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            RefreshView.IsRefreshing = true;

            var users = await _userService.GetUsersAsync();
            UsersCollection.ItemsSource = users;
            CountLabel.Text = $"{users.Count} {(users.Count == 1 ? "profile" : "profiles")}";
        }
        catch (Exception ex)
        {
            CountLabel.Text = "Unable to load profiles";
            await DisplayAlert(
                "Unable to load profiles",
                $"Please make sure the API is running and the phone is connected to the same network.\n\n{ex.Message}",
                "OK");
        }
        finally
        {
            RefreshView.IsRefreshing = false;
        }
    }

    private async void OnRefresh(object? sender, EventArgs e)
    {
        await LoadUsersAsync();
    }

    private async void OnRefreshClicked(object? sender, EventArgs e)
    {
        await LoadUsersAsync();
    }
}
