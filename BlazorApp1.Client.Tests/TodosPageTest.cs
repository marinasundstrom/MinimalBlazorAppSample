using BlazorApp1.Client.Todos;

using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp1.Client.Tests;

public class TodosPageTest
{
    [Fact]
    public async Task TodosLoaded()
    {
        // Arrange

        using var ctx = new TestContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;

        ctx.Services.AddMudServices();

        ctx.Services.AddLocalization();

        var fakeTodosClient = Substitute.For<ITodosClient>();
        fakeTodosClient.GetTodosAsync(null, null, null, null, default, default)
            .ReturnsForAnyArgs(t => new ItemsResultOfTodo()
            {
                Items = new[]
                {
                    new Todo
                    {
                        Id = Guid.NewGuid(),
                        Title = "Item 1",
                        IsCompleted = true,
                        Completed =  DateTimeOffset.Now.AddMinutes(-2),
                        Created = DateTimeOffset.Now.AddMinutes(-3)
                    },
                    new Todo
                    {
                        Id = Guid.NewGuid(),
                        Title = "Item 2",
                        IsCompleted = false,
                        Completed = null,
                        Created = DateTimeOffset.Now.AddMinutes(-1)
                    },
                    new Todo
                    {
                        Id = Guid.NewGuid(),
                        Title = "Item 3",
                        IsCompleted = false,
                        Completed = null,
                        Created = DateTimeOffset.Now
                    }
                },
                TotalItems = 3
            });

        ctx.Services.AddSingleton(fakeTodosClient);

        var cut = ctx.RenderComponent<TodosPage>();

        // Act

        //cut.Find("button").Click();

        // Assert

        cut.WaitForState(() => cut.Find("tr") != null);

        await fakeTodosClient
            .ReceivedWithAnyArgs()
            .GetTodosAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<SortDirection?>(), Arg.Any<CancellationToken>());

        int expectedNoOfTr = 4; // incl <td> in <thead>

        cut.FindAll("tr").Count.Should().Be(expectedNoOfTr);
    }
}