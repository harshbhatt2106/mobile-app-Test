using Microsoft.Extensions.Logging;

namespace MobileAppTest;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>();


        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<Services.UserService>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
