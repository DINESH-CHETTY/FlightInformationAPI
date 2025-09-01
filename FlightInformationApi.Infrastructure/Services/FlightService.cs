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

    public async Task<FlightDto?> GetFlightByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving flight with ID: {FlightId}", id);

        var flight = await _flightRepository.GetByIdAsync(id);

        if (flight == null)
        {
            _logger.LogWarning("Flight with ID {FlightId} not found", id);
        }
        else
        {
            _logger.LogDebug("Retrieved flight {FlightNumber} for ID {FlightId}", flight.FlightNumber, id);
        }

        return flight?.ToDto();
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

    public async Task<FlightDto?> UpdateFlightAsync(int id, UpdateFlightDto updateFlightDto)
    {
        _logger.LogInformation("Updating flight with ID {FlightId}. New data: {FlightNumber}",
            id, updateFlightDto.FlightNumber);

        var flight = updateFlightDto.ToEntity();
        var updatedFlight = await _flightRepository.UpdateAsync(id, flight);

        if (updatedFlight != null)
        {
            _logger.LogInformation("Flight with ID {FlightId} updated successfully: {FlightNumber} - Status: {Status}",
                id, updatedFlight.FlightNumber, updatedFlight.Status);
        }
        else
        {
            _logger.LogWarning("Flight with ID {FlightId} not found for update", id);
        }

        return updatedFlight?.ToDto();
    }

    public async Task<bool> DeleteFlightAsync(int id)
    {
        _logger.LogInformation("Attempting to delete flight with ID {FlightId}", id);

        var result = await _flightRepository.DeleteAsync(id);

        if (result)
        {
            _logger.LogInformation("Flight with ID {FlightId} deleted successfully", id);
        }
        else
        {
            _logger.LogWarning("Flight with ID {FlightId} not found for deletion", id);
        }

        return result;
    }

    public async Task<IEnumerable<FlightDto>> SearchFlightsAsync(FlightSearchDto searchDto)
    {
        _logger.LogInformation("Searching flights with criteria - Airline: {Airline}, DepartureAirport: {DepartureAirport}, ArrivalAirport: {ArrivalAirport}, Status: {Status}",
            searchDto.Airline ?? "Any",
            searchDto.DepartureAirport ?? "Any",
            searchDto.ArrivalAirport ?? "Any",
            searchDto.Status ?? "Any");

        var flights = await _flightRepository.SearchAsync(searchDto);

        _logger.LogInformation("Search completed. Found {FlightCount} matching flights", flights.Count());
        return flights.Select(f => f.ToDto());
    }
}