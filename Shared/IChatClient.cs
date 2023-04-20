namespace BlazorApp1.Shared;

public interface IChatClient
{
    Task ReceiveMessage(string user, string message);
}