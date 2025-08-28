using System.ComponentModel.DataAnnotations;

namespace FlightInformationApi.Core.Entities;

public class Flight
{
    public int Id { get; set; }

    [Required]
    [StringLength(10)]
    public string FlightNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Airline { get; set; } = string.Empty;

    [Required]
    [StringLength(5)]
    public string DepartureAirport { get; set; } = string.Empty;

    [Required]
    [StringLength(5)]
    public string ArrivalAirport { get; set; } = string.Empty;

    [Required]
    public DateTime DepartureTime { get; set; }

    [Required]
    public DateTime ArrivalTime { get; set; }

    [Required]
    public FlightStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}