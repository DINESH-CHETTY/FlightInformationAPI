using FlightInformationApi.Core.Entities;
using FlightInformationApi.Core.DTOs;

namespace FlightInformationApi.Core.Interfaces;

public interface IFlightRepository
{
    Task<IEnumerable<Flight>> GetAllAsync();
    Task<Flight> CreateAsync(Flight flight);
}