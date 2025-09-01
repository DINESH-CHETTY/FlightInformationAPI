using FlightInformationApi.Core.DTOs;
using FlightInformationApi.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace FlightInformationApi.Tests.Integration;

public class FlightsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly InMemoryDatabaseRoot _dbRoot = new();
    private readonly HttpClient _client;

    public FlightsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<FlightDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<FlightDbContext>(options =>
                    options.UseInMemoryDatabase("FlightsTestDb", _dbRoot));
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetFlights_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/flights");

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
    }

    [Fact]
    public async Task CreateFlight_ValidFlight_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateFlightDto(
            "NZ999",
            "Air New Zealand",
            "AKL",
            "CHC",
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(3),
            "Scheduled"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/flights", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdFlight = await response.Content.ReadFromJsonAsync<FlightDto>();
        createdFlight.Should().NotBeNull();
        createdFlight!.FlightNumber.Should().Be("NZ999");
    }

    [Fact]
    public async Task CreateFlight_InvalidFlight_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateFlightDto(
            "", // Invalid empty flight number
            "Air New Zealand",
            "AKL",
            "CHC",
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(3),
            "Scheduled"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/flights", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetFlightById_NonExistingId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/flights/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchFlights_ByAirline_ReturnsMatchingFlights()
    {
        // Arrange
        var createDto = new CreateFlightDto(
            "NZ777",
            "Air New Zealand",
            "AKL",
            "CHC",
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(3),
            "Scheduled"
        );

        var created = await _client.PostAsJsonAsync("/api/flights", createDto);
        var createdFlight = await created.Content.ReadFromJsonAsync<FlightDto>();
        // Act
        var response = await _client.GetAsync("/api/flights/search?airline=Air%20New%20Zealand");

        // Assert
        response.EnsureSuccessStatusCode();
        var flights = await response.Content.ReadFromJsonAsync<List<FlightDto>>();
        flights.Should().NotBeNull();
        flights!.Should().HaveCountGreaterThan(0);
        flights.All(f => f.Airline.Contains("Air New Zealand")).Should().BeTrue();
    }
}