using FlightInformationApi.Core.DTOs;
using FlightInformationApi.Core.Entities;
using FlightInformationApi.Core.Interfaces;
using FlightInformationApi.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlightInformationApi.Tests.Services;

public class FlightServiceTests
{
    private readonly Mock<IFlightRepository> _mockRepository;
    private readonly Mock<ILogger<FlightService>> _mockLogger;
    private readonly FlightService _service;

    public FlightServiceTests()
    {
        _mockRepository = new Mock<IFlightRepository>();
        _mockLogger = new Mock<ILogger<FlightService>>();
        _service = new FlightService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllFlightsAsync_ReturnsFlightDtos()
    {
        // Arrange
        var flights = new List<Flight>
        {
            new()
            {
                Id = 1,
                FlightNumber = "NZ123",
                Airline = "Air New Zealand",
                DepartureAirport = "AKL",
                ArrivalAirport = "CHC",
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(3),
                Status = FlightStatus.Scheduled
            }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(flights);

        // Act
        var result = await _service.GetAllFlightsAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().FlightNumber.Should().Be("NZ123");
        result.First().Status.Should().Be("Scheduled");
    }

    [Fact]
    public async Task GetFlightByIdAsync_ExistingId_ReturnsFlightDto()
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

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(flight);

        // Act
        var result = await _service.GetFlightByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.FlightNumber.Should().Be("NZ123");
    }

    [Fact]
    public async Task GetFlightByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Flight?)null);

        // Act
        var result = await _service.GetFlightByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateFlightAsync_ValidDto_ReturnsCreatedFlightDto()
    {
        // Arrange
        var createDto = new CreateFlightDto(
            "NZ123",
            "Air New Zealand",
            "AKL",
            "CHC",
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(3),
            "Scheduled"
        );

        var createdFlight = new Flight
        {
            Id = 1,
            FlightNumber = "NZ123",
            Airline = "Air New Zealand",
            DepartureAirport = "AKL",
            ArrivalAirport = "CHC",
            DepartureTime = createDto.DepartureTime,
            ArrivalTime = createDto.ArrivalTime,
            Status = FlightStatus.Scheduled
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Flight>())).ReturnsAsync(createdFlight);

        // Act
        var result = await _service.CreateFlightAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.FlightNumber.Should().Be("NZ123");
        result.Id.Should().Be(1);

        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Flight>()), Times.Once);
    }

    [Fact]
    public async Task UpdateFlightAsync_ExistingFlight_ReturnsUpdatedFlightDto()
    {
        // Arrange
        var updateDto = new UpdateFlightDto(
            "NZ124",
            "Air New Zealand",
            "AKL",
            "WLG",
            DateTime.UtcNow.AddHours(3),
            DateTime.UtcNow.AddHours(4),
            "Delayed"
        );

        var updatedFlight = new Flight
        {
            Id = 1,
            FlightNumber = "NZ124",
            Airline = "Air New Zealand",
            DepartureAirport = "AKL",
            ArrivalAirport = "WLG",
            DepartureTime = updateDto.DepartureTime,
            ArrivalTime = updateDto.ArrivalTime,
            Status = FlightStatus.Delayed
        };

        _mockRepository.Setup(r => r.UpdateAsync(1, It.IsAny<Flight>())).ReturnsAsync(updatedFlight);

        // Act
        var result = await _service.UpdateFlightAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.FlightNumber.Should().Be("NZ124");
        result.Status.Should().Be("Delayed");

        _mockRepository.Verify(r => r.UpdateAsync(1, It.IsAny<Flight>()), Times.Once);
    }

    [Fact]
    public async Task DeleteFlightAsync_ExistingFlight_ReturnsTrue()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteFlightAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task SearchFlightsAsync_ValidCriteria_ReturnsMatchingFlights()
    {
        // Arrange
        var searchDto = new FlightSearchDto(Airline: "Air New Zealand");
        var flights = new List<Flight>
        {
            new()
            {
                Id = 1,
                FlightNumber = "NZ123",
                Airline = "Air New Zealand",
                DepartureAirport = "AKL",
                ArrivalAirport = "CHC",
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(3),
                Status = FlightStatus.Scheduled
            }
        };

        _mockRepository.Setup(r => r.SearchAsync(searchDto)).ReturnsAsync(flights);

        // Act
        var result = await _service.SearchFlightsAsync(searchDto);

        // Assert
        result.Should().HaveCount(1);
        result.First().Airline.Should().Be("Air New Zealand");

        _mockRepository.Verify(r => r.SearchAsync(searchDto), Times.Once);
    }
}