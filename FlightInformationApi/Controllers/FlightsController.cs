using Microsoft.AspNetCore.Mvc;
using FlightInformationApi.Core.DTOs;
using FlightInformationApi.Core.Interfaces;
using FluentValidation;

namespace FlightInformationApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FlightsController : ControllerBase
{
    private readonly IFlightService _flightService;
    private readonly IValidator<CreateFlightDto> _createValidator;


    public FlightsController(
        IFlightService flightService,
        IValidator<CreateFlightDto> createValidator)
    {
        _flightService = flightService;
        _createValidator = createValidator;
    }

    /// <summary>
    /// Retrieves all flights
    /// </summary>
    /// <returns>List of all flights</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FlightDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FlightDto>>> GetAllFlights()
    {
        var flights = await _flightService.GetAllFlightsAsync();
        return Ok(flights);
    }

    /// <summary>
    /// Retrieves a specific flight by ID
    /// </summary>
    /// <param name="id">Flight ID</param>
    /// <returns>Flight details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FlightDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlightDto>> GetFlightById(int id)
    {
        var flight = await _flightService.GetFlightByIdAsync(id);

        if (flight == null)
        {
            return NotFound($"Flight with ID {id} not found.");
        }

        return Ok(flight);
    }

    /// <summary>
    /// Creates a new flight
    /// </summary>
    /// <param name="createFlightDto">Flight creation data</param>
    /// <returns>Created flight</returns>
    [HttpPost]
    [ProducesResponseType(typeof(FlightDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FlightDto>> CreateFlight([FromBody] CreateFlightDto createFlightDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createFlightDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdFlight = await _flightService.CreateFlightAsync(createFlightDto);

        return CreatedAtAction(
            nameof(GetFlightById),
            new { id = createdFlight.Id },
            createdFlight);
    }
}