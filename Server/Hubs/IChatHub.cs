namespace BlazorApp1.Server.Hubs;

public interface IChatHub
{
    Task ReceiveMessage(string  user, string message);
}