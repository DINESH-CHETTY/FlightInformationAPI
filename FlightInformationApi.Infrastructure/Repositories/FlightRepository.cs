using Microsoft.EntityFrameworkCore;
using FlightInformationApi.Core.Entities;
using FlightInformationApi.Core.Interfaces;
using FlightInformationApi.Core.DTOs;
using FlightInformationApi.Infrastructure.Data;

namespace FlightInformationApi.Infrastructure.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly FlightDbContext _context;

    public FlightRepository(FlightDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Flight>> GetAllAsync()
    {
        return await _context.Flights
            .OrderBy(f => f.DepartureTime)
            .ToListAsync();
    }
    public async Task<Flight?> GetByIdAsync(int id)
    {
        return await _context.Flights.FindAsync(id);
    }

    public async Task<Flight> CreateAsync(Flight flight)
    {
        flight.CreatedAt = DateTime.UtcNow;
        flight.UpdatedAt = DateTime.UtcNow;

        _context.Flights.Add(flight);
        await _context.SaveChangesAsync();
        return flight;
    }
}