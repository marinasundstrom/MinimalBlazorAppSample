﻿using System.Data;
using System.Threading;

using Asp.Versioning.Builder;

using BlazorApp1.Server.Data;
using BlazorApp1.Server.Models;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Server.Endpoints;

public static class TodosEndpoints
{
    public static WebApplication MapTodosEndpoints(this WebApplication app)
    {
        var subscriptions = app.NewVersionedApi("Todos");

        MapVersion1(subscriptions);

        return app;
    }

    private static void MapVersion1(IVersionedEndpointRouteBuilder builder)
    {
        var routeGroup = builder
            .MapGroup("/v{version:apiVersion}/Todos")
            .WithTags("Todos")
            .HasApiVersion(1, 0)
            .WithOpenApi();

        routeGroup
            .MapGet("/", GetTodos)
            .WithName("Todos_GetTodos")
            .Produces<IEnumerable<Todo>>(StatusCodes.Status200OK);

        routeGroup
            .MapGet("/{id}", GetTodoById)
            .WithName("Todos_GetTodoById")
            .Produces<Todo>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        routeGroup
            .MapPost("/", AddTodo)
            .WithName("Todos_AddTodo")
            .Produces<Todo>(StatusCodes.Status201Created);

        routeGroup
            .MapPut("/{id}", UpdateTodo)
            .WithName("Todos_UpdateTodo")
            .Produces<Todo>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        routeGroup
            .MapPost("/{id}/Complete", MarkTodoAsComplete)
            .WithName("Todos_MarkTodoAsComplete")
            .Produces<Todo>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        routeGroup
            .MapDelete("/{id}", DeleteTodo)
            .WithName("Todos_DeleteTodo")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> GetTodos(DataContext context, CancellationToken cancellationToken)
    {
        var todos = await context.Todos.ToArrayAsync(cancellationToken);

        if (!todos.Any())
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Ok(todos);
    }

    public static async Task<IResult> GetTodoById(Guid id, DataContext context, CancellationToken cancellationToken)
    {
        var todo = await context.Todos.FirstOrDefaultAsync(todo => todo.Id == id, cancellationToken);

        if (todo is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(todo);
    }

    public static async Task<IResult> AddTodo(AddTodoRequest request, DataContext context, CancellationToken cancellationToken)
    {
        var todo = new Todo(request.Title);

        context.Todos.Add(todo);

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/v1/Todos/{todo.Id}", todo);
    }

    public static async Task<IResult> UpdateTodo(Guid id, UpdateTodoRequest request, DataContext context, CancellationToken cancellationToken)
    {
        var todo = await context.Todos.FirstOrDefaultAsync(todo => todo.Id == id, cancellationToken);

        if (todo is null)
        {
            return TypedResults.NotFound();
        }

        todo.Title = request.Title;

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(todo);
    }

    public static async Task<IResult> MarkTodoAsComplete(Guid id, DataContext context, CancellationToken cancellationToken)
    {
        var todo = await context.Todos.FirstOrDefaultAsync(todo => todo.Id == id, cancellationToken);

        if (todo is null)
        {
            return TypedResults.NotFound();
        }

        todo.Complete();

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(todo);
    }

    public static async Task<IResult> DeleteTodo(Guid id, DataContext context, CancellationToken cancellationToken)
    {
        var removed = await context.Todos
            .Where(todo => todo.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        if (removed == 0)
        {
            return Results.NotFound();
        }

        return TypedResults.Ok();
    }

    public sealed record AddTodoRequest(string Title);

    public sealed record UpdateTodoRequest(string Title);
}