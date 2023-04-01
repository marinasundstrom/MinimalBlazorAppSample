namespace BlazorApp1.Server.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(string user, string message);
}