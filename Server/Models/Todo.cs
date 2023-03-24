using System;
using BlazorApp1.Client;

namespace BlazorApp1.Server.Models;

public sealed class Todo
{
    private string title = default!;

    private Todo() { }

    public Todo(string title)
    {
        Title = title;

        Created = DateTimeOffset.UtcNow;
        Updated = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Title
    {
        get => title;
        set
        {
            title = value;

            Updated = DateTimeOffset.UtcNow;
        }
    }

    public DateTimeOffset Created { get; private set; }

    public DateTimeOffset Updated { get; private set; }

    public bool IsCompleted { get; private set; }

    public DateTimeOffset? Completed { get; private set; }

    public void Complete()
    {
        IsCompleted = true;

        Completed = DateTimeOffset.UtcNow;
        Updated = DateTimeOffset.UtcNow;
    }
}

