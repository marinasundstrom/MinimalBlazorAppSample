using System.Runtime.InteropServices.JavaScript;

using BlazorApp1.Shared;

using Microsoft.AspNetCore.SignalR.Client;

using MudBlazor;

namespace BlazorApp1.Client.Pages;

public partial class Chat : IChatClient
{
    private IChatHub hubProxy = default!;
    private HubConnection? hubConnection;
    readonly CancellationTokenSource cts = new();

    protected override async Task OnInitializedAsync()
    {   
        await JSHost.ImportAsync("Chat", 
            "../Pages/Chat.razor.js");

        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/chathub"))
            .Build();

        hubConnection.Closed += (exc) => 
        {
            Snackbar.Add("Connection closed", Severity.Info);
            return Task.CompletedTask;
        };

        hubConnection.Reconnecting += (exc) => 
        {
            Snackbar.Add("Reconnecting...", Severity.Info);
            return Task.CompletedTask;
        };

        hubConnection.Reconnected += (message) => 
        {
            Snackbar.Add("Reconnected", Severity.Info);
            return Task.CompletedTask;
        };

        hubProxy = hubConnection.ServerProxy<IChatHub>();
        _ = hubConnection.ClientRegistration<IChatClient>(this);

        try
        {
            await hubConnection.StartAsync(cts.Token);

            Snackbar.Add("Connected", Severity.Info);
        }
        catch (TaskCanceledException)
        {

        }
        catch
        {
            Snackbar.Add("Unable to connect", Severity.Error);
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

        cts.Cancel();
        cts.Dispose();
    }

    [JSImport("playSound", "Chat")]
    internal static partial Task PlaySound();
}