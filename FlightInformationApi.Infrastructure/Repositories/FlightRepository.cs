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

    public async Task<Flight?> UpdateAsync(int id, Flight flight)
    {
        var existingFlight = await _context.Flights.FindAsync(id);
        if (existingFlight == null)
            return null;

        existingFlight.FlightNumber = flight.FlightNumber;
        existingFlight.Airline = flight.Airline;
        existingFlight.DepartureAirport = flight.DepartureAirport;
        existingFlight.ArrivalAirport = flight.ArrivalAirport;
        existingFlight.DepartureTime = flight.DepartureTime;
        existingFlight.ArrivalTime = flight.ArrivalTime;
        existingFlight.Status = flight.Status;
        existingFlight.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingFlight;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var flight = await _context.Flights.FindAsync(id);
        if (flight == null)
            return false;

        _context.Flights.Remove(flight);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Flight>> SearchAsync(FlightSearchDto searchDto)
    {
        var query = _context.Flights.AsQueryable();

        if (!string.IsNullOrEmpty(searchDto.Airline))
        {
            query = query.Where(f => f.Airline.Contains(searchDto.Airline));
        }

        if (!string.IsNullOrEmpty(searchDto.DepartureAirport))
        {
            query = query.Where(f => f.DepartureAirport == searchDto.DepartureAirport);
        }

        if (!string.IsNullOrEmpty(searchDto.ArrivalAirport))
        {
            query = query.Where(f => f.ArrivalAirport == searchDto.ArrivalAirport);
        }

        if (searchDto.DepartureFromDate.HasValue)
        {
            query = query.Where(f => f.DepartureTime >= searchDto.DepartureFromDate.Value);
        }

        if (searchDto.DepartureToDate.HasValue)
        {
            query = query.Where(f => f.DepartureTime <= searchDto.DepartureToDate.Value);
        }

        if (!string.IsNullOrEmpty(searchDto.Status) &&
            Enum.TryParse<FlightStatus>(searchDto.Status, true, out var status))
        {
            query = query.Where(f => f.Status == status);
        }

        return await query.OrderBy(f => f.DepartureTime).ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Flights.AnyAsync(f => f.Id == id);
    }
}