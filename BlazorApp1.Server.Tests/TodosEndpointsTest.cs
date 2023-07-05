using BlazorApp1.Server.Endpoints;
using BlazorApp1.Server.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

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

        var mockLinkGenerator = new MockLinkGenerator();

        // Act

        var result = await TodosEndpoints.AddTodo(request, context, mockLinkGenerator, default);

        //Assert
        result.Result.Should().BeOfType<Created<Todo>>();
    }
}

public sealed class MockLinkGenerator : LinkGenerator
{
    public override string? GetPathByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values, RouteValueDictionary? ambientValues = null, PathString? pathBase = null, FragmentString fragment = default, LinkOptions? options = null)
    {
        return "https://test";
    }

    public override string? GetPathByAddress<TAddress>(TAddress address, RouteValueDictionary values, PathString pathBase = default, FragmentString fragment = default, LinkOptions? options = null)
    {
        return "https://test";
    }

    public override string? GetUriByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values, RouteValueDictionary? ambientValues = null, string? scheme = null, HostString? host = null, PathString? pathBase = null, FragmentString fragment = default, LinkOptions? options = null)
    {
        return "https://test";
    }

    public override string? GetUriByAddress<TAddress>(TAddress address, RouteValueDictionary values, string? scheme, HostString host, PathString pathBase = default, FragmentString fragment = default, LinkOptions? options = null)
    {
        return "https://test";
    }
}