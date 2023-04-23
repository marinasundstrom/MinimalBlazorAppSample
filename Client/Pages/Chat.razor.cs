using BlazorApp1.Shared;

using Microsoft.AspNetCore.SignalR.Client;

using MudBlazor;

namespace BlazorApp1.Client.Pages;

public partial class Chat : IChatClient
{
    private IChatHub hubProxy = default!;
    private HubConnection? hubConnection;

    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/chathub"))
            .Build();

        hubProxy = hubConnection.ServerProxy<IChatHub>();
        _ = hubConnection.ClientRegistration<IChatClient>(this);

        try
        {
            await hubConnection.StartAsync();
        }
        catch
        {
            Snackbar.Add("Could not connect", Severity.Error);
        }
    }

    public bool IsConnected =>
        hubConnection?.State == HubConnectionState.Connected;

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}