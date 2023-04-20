namespace BlazorApp1.Shared;

public interface IChatHub
{
    Task SendMessage(string user, string message);
}