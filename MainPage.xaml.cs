using System.Collections.ObjectModel;
using MobileAppTest.Models;
using MobileAppTest.Services;

namespace MobileAppTest;

public partial class MainPage : ContentPage
{
    private readonly UserService _userService;
    private readonly ObservableCollection<User> _users = new();
    private int? _editingUserId;

    public MainPage(UserService userService)
    {
        InitializeComponent();
        _userService = userService;
        UsersCollection.ItemsSource = _users;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            RefreshView.IsRefreshing = true;
            var users = await _userService.GetUsersAsync();

            _users.Clear();
            foreach (var user in users)
                _users.Add(user);

            CountLabel.Text = $"{_users.Count} {(_users.Count == 1 ? "profile" : "profiles")}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Connection problem",
                $"Could not load profiles. Make sure IIS is running and the phone is on the same Wi-Fi network.\n\n{ex.Message}",
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

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await DisplayAlert("Missing name", "Please enter a full name.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || !EmailEntry.Text.Contains('@'))
        {
            await DisplayAlert("Invalid email", "Please enter a valid email address.", "OK");
            return;
        }

        if (!int.TryParse(AgeEntry.Text, out var age) || age < 18 || age > 100)
        {
            await DisplayAlert("Invalid age", "Please enter an age between 18 and 100.", "OK");
            return;
        }

        SaveButton.IsEnabled = false;

        try
        {
            var user = new User
            {
                Id = _editingUserId ?? 0,
                Name = NameEntry.Text.Trim(),
                Email = EmailEntry.Text.Trim(),
                Age = age
            };

            if (_editingUserId.HasValue)
                await _userService.UpdateUserAsync(user);
            else
                await _userService.CreateUserAsync(user);

            ResetForm();
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Save failed", ex.Message, "OK");
        }
        finally
        {
            SaveButton.IsEnabled = true;
        }
    }

    private void OnEditClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not User user)
            return;

        _editingUserId = user.Id;
        NameEntry.Text = user.Name;
        EmailEntry.Text = user.Email;
        AgeEntry.Text = user.Age.ToString();
        FormTitle.Text = "Edit profile";
        SaveButton.Text = "Save Changes";
        CancelButton.IsVisible = true;
        ScrollToForm();
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not User user)
            return;

        var confirmed = await DisplayAlert(
            "Delete profile",
            $"Delete {user.Name}'s profile? This cannot be undone.",
            "Delete",
            "Cancel");

        if (!confirmed)
            return;

        try
        {
            await _userService.DeleteUserAsync(user.Id);
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Delete failed", ex.Message, "OK");
        }
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        ResetForm();
    }

    private void ResetForm()
    {
        _editingUserId = null;
        NameEntry.Text = string.Empty;
        EmailEntry.Text = string.Empty;
        AgeEntry.Text = string.Empty;
        FormTitle.Text = "Add a profile";
        SaveButton.Text = "Add Profile";
        CancelButton.IsVisible = false;
    }

    private async void ScrollToForm()
    {
        await Task.Delay(50);
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await ScrollViewToTopAsync();
        });
    }

    private Task ScrollViewToTopAsync()
    {
        // The form is already near the top; keeping this method separate makes
        // it easy to add precise scrolling when the page grows.
        return Task.CompletedTask;
    }
}
