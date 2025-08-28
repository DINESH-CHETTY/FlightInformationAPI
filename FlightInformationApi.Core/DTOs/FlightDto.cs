namespace FlightInformationApi.Core.DTOs;

public record FlightDto(
    int Id,
    string FlightNumber,
    string Airline,
    string DepartureAirport,
    string ArrivalAirport,
    DateTime DepartureTime,
    DateTime ArrivalTime,
    string Status
);

public record CreateFlightDto(
    string FlightNumber,
    string Airline,
    string DepartureAirport,
    string ArrivalAirport,
    DateTime DepartureTime,
    DateTime ArrivalTime,
    string Status
);

public record UpdateFlightDto(
    string FlightNumber,
    string Airline,
    string DepartureAirport,
    string ArrivalAirport,
    DateTime DepartureTime,
    DateTime ArrivalTime,
    string Status
);

public record FlightSearchDto(
    string? Airline = null,
    string? DepartureAirport = null,
    string? ArrivalAirport = null,
    DateTime? DepartureFromDate = null,
    DateTime? DepartureToDate = null,
    string? Status = null
);