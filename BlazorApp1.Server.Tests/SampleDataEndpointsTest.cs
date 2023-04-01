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
        var okResult = (Ok<Endpoints.SampleDataEndpoints.WeatherForecast[]>)SampleDataEndpoints.GetForecast(startDate);

        //Assert
        okResult.StatusCode
            .Should().Be(StatusCodes.Status200OK);

        var weatherForecast = okResult.Value;

        weatherForecast.Should().NotBeEmpty();
    }
}