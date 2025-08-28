using FlightInformationApi.Core.DTOs;
using FlightInformationApi.Core.Interfaces;
using FlightInformationApi.Core.Mappings;
using Microsoft.Extensions.Logging;

namespace FlightInformationApi.Infrastructure.Services;

public class FlightService : IFlightService
{
    private readonly IFlightRepository _flightRepository;
    private readonly ILogger<FlightService> _logger;

    public FlightService(IFlightRepository flightRepository, ILogger<FlightService> logger)
    {
        _flightRepository = flightRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<FlightDto>> GetAllFlightsAsync()
    {
        _logger.LogInformation("Retrieving all flights");

        var flights = await _flightRepository.GetAllAsync();

        _logger.LogInformation("Retrieved {FlightCount} flights", flights.Count());
        return flights.Select(f => f.ToDto());
    }

    public async Task<FlightDto> CreateFlightAsync(CreateFlightDto createFlightDto)
    {
        _logger.LogInformation("Creating new flight {FlightNumber} from {DepartureAirport} to {ArrivalAirport}",
            createFlightDto.FlightNumber, createFlightDto.DepartureAirport, createFlightDto.ArrivalAirport);

        var flight = createFlightDto.ToEntity();
        var createdFlight = await _flightRepository.CreateAsync(flight);

        _logger.LogInformation("Flight created successfully with ID {FlightId}: {FlightNumber}",
            createdFlight.Id, createdFlight.FlightNumber);

        return createdFlight.ToDto();
    }
}