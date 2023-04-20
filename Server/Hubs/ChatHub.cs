using BlazorApp1.Shared;

using Microsoft.AspNetCore.SignalR;

namespace BlazorApp1.Server.Hubs;

public sealed class ChatHub : Hub<IChatClient>, IChatHub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.ReceiveMessage(user, message);
    }
}