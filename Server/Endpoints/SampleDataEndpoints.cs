using System.Data;

using Asp.Versioning.Builder;

using Microsoft.AspNetCore.Http.HttpResults;

using static BlazorApp1.Server.ApiVersions;
using static Microsoft.AspNetCore.Http.TypedResults;

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

        MapVersion2(subscriptions);

        return app;
    }

    private static void MapVersion1(IVersionedEndpointRouteBuilder builder)
    {
        var routeGroup = builder
            .MapGroup("/v{version:apiVersion}/WeatherForecast")
            .WithTags("SampleData")
            .HasApiVersion(V1)
            .WithOpenApi();

        routeGroup
            .MapGet("/", GetForecast)
            .WithName("SampleData_GetForecast")
            .Produces<IEnumerable<WeatherForecast>>(StatusCodes.Status200OK);
    }

    private static void MapVersion2(IVersionedEndpointRouteBuilder builder)
    {
        var routeGroup = builder
            .MapGroup("/v{version:apiVersion}/WeatherForecast")
            .WithTags("SampleData")
            .HasApiVersion(V2)
            .WithOpenApi();

        routeGroup
            .MapGet("/", GetForecastV2)
            .WithName("SampleData_GetForecastV2")
            .Produces<ItemsResult<WeatherForecast>>(StatusCodes.Status200OK);
    }

    public static Ok<WeatherForecast[]> GetForecast(DateTime startDate)
    {
        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray();

        return Ok(forecasts);
    }

    public static Ok<ItemsResult<WeatherForecast>> GetForecastV2(DateTime? startDate = null, int page = 1, int pageSize = 7)
    {
        startDate = startDate ?? DateTime.UtcNow;

        var forecasts = Enumerable.Range((page - 1) * pageSize, pageSize).Select(index =>
        {
            return new WeatherForecast
            {
                Date = startDate.GetValueOrDefault().AddDays(-index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };
        });

        return Ok(new ItemsResult<WeatherForecast>(forecasts, 100));
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string? Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}