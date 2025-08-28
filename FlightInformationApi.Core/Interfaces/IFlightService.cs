using FlightInformationApi.Core.DTOs;

namespace FlightInformationApi.Core.Interfaces;

public interface IFlightService
{
    Task<IEnumerable<FlightDto>> GetAllFlightsAsync();
    Task<FlightDto> CreateFlightAsync(CreateFlightDto createFlightDto);
}