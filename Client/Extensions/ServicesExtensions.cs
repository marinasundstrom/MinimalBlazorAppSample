using BlazorApp1.Client.Services;
using BlazorApp1.Client.Services.UserPreferences;

using Blazored.LocalStorage;

using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp1.Client.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddBlazoredLocalStorage();

        services.AddScoped<LayoutService>();

        services.AddScoped<IUserPreferencesService, UserPreferencesService>();

        return services;
    }
}