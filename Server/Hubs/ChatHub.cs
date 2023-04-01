using Microsoft.AspNetCore.SignalR;

namespace BlazorApp1.Server.Hubs;

public interface IChatHub
{
    Task SendMessage(string user, string message);
}

public sealed class ChatHub : Hub<IChatClient>, IChatHub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.ReceiveMessage(user, message);
    }
}