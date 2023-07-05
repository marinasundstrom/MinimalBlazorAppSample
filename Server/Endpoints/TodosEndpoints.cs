using System.Data;

using Asp.Versioning.Builder;

using BlazorApp1.Server.Data;
using BlazorApp1.Server.Models;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

using static BlazorApp1.Server.ApiVersions;
using static Microsoft.AspNetCore.Http.TypedResults;

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
            .HasApiVersion(V1)
            .HasApiVersion(V2)
            .WithOpenApi();

        routeGroup
            .MapGet("/", GetTodos)
            .WithName("Todos_GetTodos")
            .Produces<ItemsResult<Todo>>(StatusCodes.Status200OK);

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

    public static async Task<Results<Ok<ItemsResult<Todo>>, NotFound>> GetTodos(DataContext context, int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (pageSize < 0)
        {
            throw new Exception("Page Size cannot be negative.");
        }

        if (pageSize > 100)
        {
            throw new Exception("Page Size must not be greater than 100.");
        }

        IQueryable<Todo> result = context.Todos
                  .AsNoTracking()
                  .AsQueryable();

        if (searchString is not null)
        {
            result = result.Where(p =>
                p.Title.ToLower().Contains(searchString.ToLower()));
        }

        var totalCount = await result.CountAsync(cancellationToken);

        if (sortBy is not null)
        {
            result = result.OrderBy(sortBy, sortDirection);
        }
        else
        {
            result = result.OrderBy(x => x.Created);
        }

        var todos = await result
            .AsSingleQuery()
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new ItemsResult<Todo>(todos, totalCount));
    }

    public static async Task<Results<Ok<Todo>, NotFound>> GetTodoById(Guid id, DataContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);

        var todo = await context.Todos.FirstOrDefaultAsync(todo => todo.Id == id, cancellationToken);

        return todo is not null ? Ok(todo) : NotFound();
    }

    public static async Task<Results<Created<Todo>, BadRequest>> AddTodo(AddTodoRequest request, DataContext context, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);

        var todo = new Todo(request.Title);

        context.Todos.Add(todo);

        await context.SaveChangesAsync(cancellationToken);

        return Created(linkGenerator.GetPathByName("Todos_GetTodoById", new { id = todo })!, todo);
    }

    public static async Task<Results<Ok<Todo>, NotFound>> UpdateTodo(Guid id, UpdateTodoRequest request, DataContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);

        var todo = await context.Todos.FirstOrDefaultAsync(todo => todo.Id == id, cancellationToken);

        if (todo is null)
        {
            return NotFound();
        }

        todo.Title = request.Title;

        await context.SaveChangesAsync(cancellationToken);

        return Ok(todo);
    }

    public static async Task<Results<Ok<Todo>, NotFound>> MarkTodoAsComplete(Guid id, DataContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);

        var todo = await context.Todos.FirstOrDefaultAsync(todo => todo.Id == id, cancellationToken);

        if (todo is null)
        {
            return NotFound();
        }

        todo.Complete();

        await context.SaveChangesAsync(cancellationToken);

        return Ok(todo);
    }

    public static async Task<Results<Ok, NotFound>> DeleteTodo(Guid id, DataContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);

        var removed = await context.Todos
            .Where(todo => todo.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return removed >= 1 ? Ok() : NotFound();
    }

    public sealed record AddTodoRequest(string Title);

    public sealed record UpdateTodoRequest(string Title);
}