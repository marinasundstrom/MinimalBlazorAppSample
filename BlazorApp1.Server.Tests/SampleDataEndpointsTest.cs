using BlazorApp1.Server.Endpoints;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BlazorApp1.Server.Tests;

public class SampleDataEndpointsTest
{
    [Fact]
    public void GetForecastReturnsOkIfSucceeded()
    {
        // Arrange
        DateTime startDate = DateTime.Now;

        // Act
        var result = SampleDataEndpoints.GetForecast(startDate);

        //Assert
        result.Should().BeOfType<Ok<Endpoints.SampleDataEndpoints.WeatherForecast[]>>();

        var okResult = result as Ok<Endpoints.SampleDataEndpoints.WeatherForecast[]>;

        var weatherForecast = okResult.Value;

        weatherForecast.Should().NotBeEmpty();
    }
}