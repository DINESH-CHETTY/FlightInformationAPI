using FlightInformationApi.Core.DTOs;
using FlightInformationApi.Core.Entities;

namespace FlightInformationApi.Core.Mappings;

public static class FlightMappingExtensions
{
    public static FlightDto ToDto(this Flight flight)
    {
        return new FlightDto(
            flight.Id,
            flight.FlightNumber,
            flight.Airline,
            flight.DepartureAirport,
            flight.ArrivalAirport,
            flight.DepartureTime,
            flight.ArrivalTime,
            flight.Status.ToString()
        );
    }

    public static Flight ToEntity(this CreateFlightDto createDto)
    {
        return new Flight
        {
            FlightNumber = createDto.FlightNumber,
            Airline = createDto.Airline,
            DepartureAirport = createDto.DepartureAirport,
            ArrivalAirport = createDto.ArrivalAirport,
            DepartureTime = createDto.DepartureTime,
            ArrivalTime = createDto.ArrivalTime,
            Status = Enum.Parse<FlightStatus>(createDto.Status, true)
        };
    }
}