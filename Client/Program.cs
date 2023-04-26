using BlazorApp1.Client;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Microsoft.Extensions.Localization;

using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

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

await builder.Build().RunAsync();