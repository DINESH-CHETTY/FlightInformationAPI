using FluentValidation;
using FlightInformationApi.Core.DTOs;
using FlightInformationApi.Core.Entities;

namespace FlightInformationApi.Core.Validators;

public class FlightSearchValidator : AbstractValidator<FlightSearchDto>
{
    public FlightSearchValidator()
    {
        RuleFor(x => x.Airline)
            .Length(2, 100)
            .When(x => !string.IsNullOrEmpty(x.Airline))
            .WithMessage("Airline name must be between 2 and 100 characters");

        RuleFor(x => x.DepartureAirport)
            .Length(3, 5)
            .Matches("^[A-Z]{3,4}$")
            .When(x => !string.IsNullOrEmpty(x.DepartureAirport))
            .WithMessage("Departure airport code must be 3-4 uppercase letters (e.g., AKL, NZAA)");

        RuleFor(x => x.ArrivalAirport)
            .Length(3, 5)
            .Matches("^[A-Z]{3,4}$")
            .When(x => !string.IsNullOrEmpty(x.ArrivalAirport))
            .WithMessage("Arrival airport code must be 3-4 uppercase letters (e.g., CHC, NZCH)");

        RuleFor(x => x.Status)
            .Must(BeAValidStatus)
            .When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status must be one of: Scheduled, Delayed, Cancelled, InAir, Landed");

        RuleFor(x => x.DepartureFromDate)
            .LessThan(x => x.DepartureToDate)
            .When(x => x.DepartureFromDate.HasValue && x.DepartureToDate.HasValue)
            .WithMessage("Departure from date must be before departure to date");

        RuleFor(x => x.DepartureFromDate)
            .GreaterThan(DateTime.UtcNow.AddYears(-1))
            .When(x => x.DepartureFromDate.HasValue)
            .WithMessage("Departure from date cannot be more than 1 year in the past");

        RuleFor(x => x.DepartureToDate)
            .LessThan(DateTime.UtcNow.AddYears(2))
            .When(x => x.DepartureToDate.HasValue)
            .WithMessage("Departure to date cannot be more than 2 years in the future");

        // Validate date range is not too wide (optional business rule)
        RuleFor(x => x)
            .Must(x => !x.DepartureFromDate.HasValue || !x.DepartureToDate.HasValue ||
                      (x.DepartureToDate.Value - x.DepartureFromDate.Value).TotalDays <= 365)
            .When(x => x.DepartureFromDate.HasValue && x.DepartureToDate.HasValue)
            .WithMessage("Date range cannot exceed 365 days");

        // At least one search criteria must be provided
        RuleFor(x => x)
            .Must(HaveAtLeastOneSearchCriteria)
            .WithMessage("At least one search criterion must be provided")
            .WithName("SearchCriteria");
    }

    private bool BeAValidStatus(string? status)
    {
        if (string.IsNullOrEmpty(status))
            return true;

        return Enum.TryParse<FlightStatus>(status, true, out _);
    }

    private bool HaveAtLeastOneSearchCriteria(FlightSearchDto searchDto)
    {
        return !string.IsNullOrWhiteSpace(searchDto.Airline) ||
               !string.IsNullOrWhiteSpace(searchDto.DepartureAirport) ||
               !string.IsNullOrWhiteSpace(searchDto.ArrivalAirport) ||
               !string.IsNullOrWhiteSpace(searchDto.Status) ||
               searchDto.DepartureFromDate.HasValue ||
               searchDto.DepartureToDate.HasValue;
    }
}