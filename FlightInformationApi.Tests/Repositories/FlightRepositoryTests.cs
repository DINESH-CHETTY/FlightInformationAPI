using FlightInformationApi.Core.DTOs;
using FlightInformationApi.Core.Entities;
using FlightInformationApi.Infrastructure.Data;
using FlightInformationApi.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlightInformationApi.Tests.Repositories;

public class FlightRepositoryTests : IDisposable
{
    private readonly FlightDbContext _context;
    private readonly FlightRepository _repository;

    public FlightRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<FlightDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FlightDbContext(options);
        _repository = new FlightRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFlights_OrderedByDepartureTime()
    {
        // Arrange
        var flights = new List<Flight>
        {
            new() {
                Id = 1,
                FlightNumber = "NZ123",
                Airline = "Air New Zealand",
                DepartureAirport = "AKL",
                ArrivalAirport = "CHC",
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(3),
                Status = FlightStatus.Scheduled
            },
            new() {
                Id = 2,
                FlightNumber = "JQ456",
                Airline = "Jetstar",
                DepartureAirport = "WLG",
                ArrivalAirport = "AKL",
                DepartureTime = DateTime.UtcNow.AddHours(1),
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                Status = FlightStatus.Scheduled
            }
        };

        _context.Flights.AddRange(flights);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().FlightNumber.Should().Be("JQ456"); // Earlier departure
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsFlight()
    {
        // Arrange
        var flight = new Flight
        {
            Id = 1,
            FlightNumber = "NZ123",
            Airline = "Air New Zealand",
            DepartureAirport = "AKL",
            ArrivalAirport = "CHC",
            DepartureTime = DateTime.UtcNow.AddHours(2),
            ArrivalTime = DateTime.UtcNow.AddHours(3),
            Status = FlightStatus.Scheduled
        };

        _context.Flights.Add(flight);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.FlightNumber.Should().Be("NZ123");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidFlight_ReturnsCreatedFlight()
    {
        // Arrange
        var flight = new Flight
        {
            FlightNumber = "NZ123",
            Airline = "Air New Zealand",
            DepartureAirport = "AKL",
            ArrivalAirport = "CHC",
            DepartureTime = DateTime.UtcNow.AddHours(2),
            ArrivalTime = DateTime.UtcNow.AddHours(3),
            Status = FlightStatus.Scheduled
        };

        // Act
        var result = await _repository.CreateAsync(flight);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateAsync_ExistingFlight_ReturnsUpdatedFlight()
    {
        // Arrange
        var originalFlight = new Flight
        {
            FlightNumber = "NZ123",
            Airline = "Air New Zealand",
            DepartureAirport = "AKL",
            ArrivalAirport = "CHC",
            DepartureTime = DateTime.UtcNow.AddHours(2),
            ArrivalTime = DateTime.UtcNow.AddHours(3),
            Status = FlightStatus.Scheduled
        };

        _context.Flights.Add(originalFlight);
        await _context.SaveChangesAsync();

        var updateFlight = new Flight
        {
            FlightNumber = "NZ124",
            Airline = "Air New Zealand",
            DepartureAirport = "AKL",
            ArrivalAirport = "WLG",
            DepartureTime = DateTime.UtcNow.AddHours(3),
            ArrivalTime = DateTime.UtcNow.AddHours(4),
            Status = FlightStatus.Delayed
        };

        // Act
        var result = await _repository.UpdateAsync(originalFlight.Id, updateFlight);

        // Assert
        result.Should().NotBeNull();
        result!.FlightNumber.Should().Be("NZ124");
        result.Status.Should().Be(FlightStatus.Delayed);
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task DeleteAsync_ExistingFlight_ReturnsTrue()
    {
        // Arrange
        var flight = new Flight
        {
            FlightNumber = "NZ123",
            Airline = "Air New Zealand",
            DepartureAirport = "AKL",
            ArrivalAirport = "CHC",
            DepartureTime = DateTime.UtcNow.AddHours(2),
            ArrivalTime = DateTime.UtcNow.AddHours(3),
            Status = FlightStatus.Scheduled
        };

        _context.Flights.Add(flight);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(flight.Id);

        // Assert
        result.Should().BeTrue();
        var deletedFlight = await _context.Flights.FindAsync(flight.Id);
        deletedFlight.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_ByAirline_ReturnsMatchingFlights()
    {
        // Arrange
        var flights = new List<Flight>
        {
            new() {
                FlightNumber = "NZ123",
                Airline = "Air New Zealand",
                DepartureAirport = "AKL",
                ArrivalAirport = "CHC",
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(3),
                Status = FlightStatus.Scheduled
            },
            new() {
                FlightNumber = "JQ456",
                Airline = "Jetstar",
                DepartureAirport = "WLG",
                ArrivalAirport = "AKL",
                DepartureTime = DateTime.UtcNow.AddHours(1),
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                Status = FlightStatus.Scheduled
            }
        };

        _context.Flights.AddRange(flights);
        await _context.SaveChangesAsync();

        var searchDto = new FlightSearchDto(Airline: "Air New Zealand");

        // Act
        var result = await _repository.SearchAsync(searchDto);

        // Assert
        result.Should().HaveCount(1);
        result.First().Airline.Should().Be("Air New Zealand");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}