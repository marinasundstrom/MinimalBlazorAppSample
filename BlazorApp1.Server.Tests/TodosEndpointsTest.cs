using BlazorApp1.Server.Endpoints;
using BlazorApp1.Server.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BlazorApp1.Server.Tests;

public class TodosEndpointsTest
{
    [Fact]
    public async void AddTodoReturnsCreatedIfSucceeded()
    {
        // Arrange
        using var context = InMemoryDb.CreateContext();

        await context.Database.EnsureCreatedAsync();

        var request = new TodosEndpoints.AddTodoRequest("test");

        // Act

        var result = await TodosEndpoints.AddTodo(request, context, default);

        //Assert
        result.Result.Should().BeOfType<Created<Todo>>();
    }
}