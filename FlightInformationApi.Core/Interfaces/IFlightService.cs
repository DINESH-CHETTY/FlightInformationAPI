using FlightInformationApi.Core.DTOs;

namespace FlightInformationApi.Core.Interfaces;

public interface IFlightService
{
    Task<IEnumerable<FlightDto>> GetAllFlightsAsync();
    Task<FlightDto?> GetFlightByIdAsync(int id);
    Task<FlightDto> CreateFlightAsync(CreateFlightDto createFlightDto);
    Task<FlightDto?> UpdateFlightAsync(int id, UpdateFlightDto updateFlightDto);
    Task<bool> DeleteFlightAsync(int id);
    Task<IEnumerable<FlightDto>> SearchFlightsAsync(FlightSearchDto searchDto);
}