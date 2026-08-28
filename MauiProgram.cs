using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using MobileAppTest.Services;

namespace MobileAppTest;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>();

        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
