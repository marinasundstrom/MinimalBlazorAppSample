using BlazorApp1.Shared;

using Microsoft.AspNetCore.SignalR.Client;

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
        await hubConnection.StartAsync();
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