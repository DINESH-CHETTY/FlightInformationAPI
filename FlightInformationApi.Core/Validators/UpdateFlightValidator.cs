using FluentValidation;
using FlightInformationApi.Core.DTOs;
using FlightInformationApi.Core.Entities;

namespace FlightInformationApi.Core.Validators;

public class UpdateFlightValidator : AbstractValidator<UpdateFlightDto>
{
    public UpdateFlightValidator()
    {
        RuleFor(x => x.FlightNumber)
            .NotEmpty()
            .Length(2, 10)
            .Matches("^[A-Z]{2}[0-9]{1,4}$")
            .WithMessage("Flight number must be in format: AA123 (2 letters followed by 1-4 numbers)");

        RuleFor(x => x.Airline)
            .NotEmpty()
            .Length(2, 100);

        RuleFor(x => x.DepartureAirport)
            .NotEmpty()
            .Length(3, 5)
            .Matches("^[A-Z]{3,4}$")
            .WithMessage("Airport code must be 3-4 uppercase letters");

        RuleFor(x => x.ArrivalAirport)
            .NotEmpty()
            .Length(3, 5)
            .Matches("^[A-Z]{3,4}$")
            .WithMessage("Airport code must be 3-4 uppercase letters");

        RuleFor(x => x.DepartureTime)
            .NotEmpty();

        RuleFor(x => x.ArrivalTime)
            .NotEmpty()
            .GreaterThan(x => x.DepartureTime)
            .WithMessage("Arrival time must be after departure time");

        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(BeAValidStatus)
            .WithMessage("Status must be one of: Scheduled, Delayed, Cancelled, InAir, Landed");

        RuleFor(x => x)
            .Must(x => x.ArrivalAirport != x.DepartureAirport)
            .WithMessage("Departure and arrival airports cannot be the same");
    }

    private bool BeAValidStatus(string status)
    {
        return Enum.TryParse<FlightStatus>(status, true, out _);
    }
}