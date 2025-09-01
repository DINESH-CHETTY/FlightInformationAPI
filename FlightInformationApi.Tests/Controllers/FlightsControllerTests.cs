using FlightInformationApi.Controllers;
using FlightInformationApi.Core.DTOs;
using FlightInformationApi.Core.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlightInformationApi.Tests.Controllers;

public class FlightsControllerTests
{
    private readonly Mock<IFlightService> _mockService;
    private readonly Mock<IValidator<CreateFlightDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateFlightDto>> _mockUpdateValidator;
    private readonly Mock<IValidator<FlightSearchDto>> _mockSearchValidator;
    private readonly FlightsController _controller;

    public FlightsControllerTests()
    {
        _mockService = new Mock<IFlightService>();
        _mockCreateValidator = new Mock<IValidator<CreateFlightDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateFlightDto>>();
        _mockSearchValidator = new Mock<IValidator<FlightSearchDto>>();

        _controller = new FlightsController(
            _mockService.Object,
            _mockCreateValidator.Object,
            _mockUpdateValidator.Object,
            _mockSearchValidator.Object);
    }

    [Fact]
    public async Task GetAllFlights_ReturnsOkWithFlights()
    {
        // Arrange
        var flights = new List<FlightDto>
        {
            new(1, "NZ123", "Air New Zealand", "AKL", "CHC",
                DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(3), "Scheduled")
        };

        _mockService.Setup(s => s.GetAllFlightsAsync()).ReturnsAsync(flights);

        // Act
        var result = await _controller.GetAllFlights();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedFlights = okResult.Value.Should().BeAssignableTo<IEnumerable<FlightDto>>().Subject;
        returnedFlights.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetFlightById_ExistingId_ReturnsOkWithFlight()
    {
        // Arrange
        var flight = new FlightDto(1, "NZ123", "Air New Zealand", "AKL", "CHC",
            DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(3), "Scheduled");

        _mockService.Setup(s => s.GetFlightByIdAsync(1)).ReturnsAsync(flight);

        // Act
        var result = await _controller.GetFlightById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedFlight = okResult.Value.Should().BeOfType<FlightDto>().Subject;
        returnedFlight.FlightNumber.Should().Be("NZ123");
    }

    [Fact]
    public async Task GetFlightById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetFlightByIdAsync(999)).ReturnsAsync((FlightDto?)null);

        // Act
        var result = await _controller.GetFlightById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateFlight_ValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateFlightDto("NZ123", "Air New Zealand", "AKL", "CHC",
            DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(3), "Scheduled");

        var createdFlight = new FlightDto(1, "NZ123", "Air New Zealand", "AKL", "CHC",
            createDto.DepartureTime, createDto.ArrivalTime, "Scheduled");

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockService.Setup(s => s.CreateFlightAsync(createDto)).ReturnsAsync(createdFlight);

        // Act
        var result = await _controller.CreateFlight(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(FlightsController.GetFlightById));
        createdResult.RouteValues!["id"].Should().Be(1);

        var returnedFlight = createdResult.Value.Should().BeOfType<FlightDto>().Subject;
        returnedFlight.FlightNumber.Should().Be("NZ123");
    }

    [Fact]
    public async Task CreateFlight_InvalidDto_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateFlightDto("", "Air New Zealand", "AKL", "CHC",
            DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(3), "Scheduled");

        var validationErrors = new List<ValidationFailure>
        {
            new("FlightNumber", "Flight number is required")
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new ValidationResult(validationErrors));

        // Act
        var result = await _controller.CreateFlight(createDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateFlight_ValidDto_ReturnsOkWithUpdatedFlight()
    {
        // Arrange
        var updateDto = new UpdateFlightDto("NZ124", "Air New Zealand", "AKL", "WLG",
            DateTime.UtcNow.AddHours(3), DateTime.UtcNow.AddHours(4), "Delayed");

        var updatedFlight = new FlightDto(1, "NZ124", "Air New Zealand", "AKL", "WLG",
            updateDto.DepartureTime, updateDto.ArrivalTime, "Delayed");

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockService.Setup(s => s.UpdateFlightAsync(1, updateDto)).ReturnsAsync(updatedFlight);

        // Act
        var result = await _controller.UpdateFlight(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedFlight = okResult.Value.Should().BeOfType<FlightDto>().Subject;
        returnedFlight.FlightNumber.Should().Be("NZ124");
        returnedFlight.Status.Should().Be("Delayed");
    }

    [Fact]
    public async Task DeleteFlight_ExistingId_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteFlightAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteFlight(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteFlight_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteFlightAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteFlight(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task SearchFlights_ValidCriteria_ReturnsOkWithFlights()
    {
        // Arrange
        var searchDto = new FlightSearchDto(Airline: "Air New Zealand");
        var flights = new List<FlightDto>
        {
            new(1, "NZ123", "Air New Zealand", "AKL", "CHC",
                DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(3), "Scheduled")
        };

        _mockSearchValidator.Setup(v => v.ValidateAsync(searchDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockService.Setup(s => s.SearchFlightsAsync(searchDto)).ReturnsAsync(flights);

        // Act
        var result = await _controller.SearchFlights(searchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedFlights = okResult.Value.Should().BeAssignableTo<IEnumerable<FlightDto>>().Subject;
        returnedFlights.Should().HaveCount(1);
        returnedFlights.First().Airline.Should().Be("Air New Zealand");
    }
}