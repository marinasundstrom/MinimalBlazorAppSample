using System.Data;

using Asp.Versioning.Builder;

using Microsoft.AspNetCore.Http.HttpResults;

namespace BlazorApp1.Server.Endpoints;

public static class SampleDataEndpoints
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public static WebApplication MapSampleDataEndpoints(this WebApplication app)
    {
        var subscriptions = app.NewVersionedApi("SampleData");

        MapVersion1(subscriptions);

        return app;
    }

    private static void MapVersion1(IVersionedEndpointRouteBuilder builder)
    {
        var routeGroup = builder
            .MapGroup("/v{version:apiVersion}/WeatherForecast")
            .WithTags("SampleData")
            .HasApiVersion(1, 0)
            .WithOpenApi();

        routeGroup
            .MapGet("/", GetForecast)
            .WithName("SampleData_GetForecast")
            .Produces<IEnumerable<WeatherForecast>>(StatusCodes.Status200OK);
    }

    public static IResult GetForecast(DateTime startDate)
    {
        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray();

        return Results.Ok(forecasts);
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string? Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}