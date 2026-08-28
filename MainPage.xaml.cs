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
            await DisplayAlert("Unable to load profiles", ex.Message, "OK");
        }
        finally
        {
            RefreshView.IsRefreshing = false;
        }
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("Add Member", "Enter member name:", "Next", "Cancel", "Name");
        if (string.IsNullOrWhiteSpace(name)) return;

        var email = await DisplayPromptAsync("Add Member", "Enter email:", "Next", "Cancel", "Email");
        if (string.IsNullOrWhiteSpace(email)) return;

        var ageText = await DisplayPromptAsync("Add Member", "Enter age:", "Save", "Cancel", "Age", keyboard: Keyboard.Numeric);
        if (!int.TryParse(ageText, out var age) || age < 0)
        {
            await DisplayAlert("Invalid age", "Please enter a valid age.", "OK");
            return;
        }

        try
        {
            var user = await _userService.CreateUserAsync(new User
            {
                Name = name.Trim(),
                Email = email.Trim(),
                Age = age
            });

            await DisplayAlert("Success", user is null ? "Member added." : $"{user.Name} was added successfully.", "OK");
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Unable to add member", ex.Message, "OK");
        }
    }

    private async void OnRefresh(object? sender, EventArgs e) => await LoadUsersAsync();

    private async void OnRefreshClicked(object? sender, EventArgs e) => await LoadUsersAsync();
}
