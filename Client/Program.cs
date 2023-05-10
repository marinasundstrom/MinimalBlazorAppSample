using System.Globalization;

using BlazorApp1.Client;
using BlazorApp1.Client.Extensions;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddLocalization();

const string HttpClientName = "Site";

builder.Services.AddHttpClient(HttpClientName, (sp, http) =>
{
    http.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient<ISampleDataClient>(HttpClientName)
.AddTypedClient<ISampleDataClient>((http, sp) => new SampleDataClient(http));

builder.Services.AddHttpClient<ITodosClient>(HttpClientName)
.AddTypedClient<ITodosClient>((http, sp) => new TodosClient(http));

builder.Services.AddServices();

var app = builder.Build();

await Localize(app.Services);

await app.RunAsync();

static async Task Localize(IServiceProvider serviceProvider)
{
    CultureInfo culture;
    var js = serviceProvider.GetRequiredService<IJSRuntime>();
    var result = await js.InvokeAsync<string>("blazorCulture.get");

    if (result != null)
    {
        culture = new CultureInfo(result);
    }
    else
    {
        culture = new CultureInfo("en-US");
        await js.InvokeVoidAsync("blazorCulture.set", "en-US");
    }

    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}