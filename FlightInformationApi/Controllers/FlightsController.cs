using Microsoft.AspNetCore.Mvc;
using FlightInformationApi.Core.DTOs;
using FlightInformationApi.Core.Interfaces;
using FluentValidation;

namespace FlightInformationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FlightsController : ControllerBase
{
    private readonly IFlightService _flightService;
    private readonly IValidator<CreateFlightDto> _createValidator;
    private readonly IValidator<UpdateFlightDto> _updateValidator;
    private readonly IValidator<FlightSearchDto> _searchValidator;

    public FlightsController(
        IFlightService flightService,
        IValidator<CreateFlightDto> createValidator,
        IValidator<UpdateFlightDto> updateValidator,
        IValidator<FlightSearchDto> searchValidator)
    {
        _flightService = flightService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _searchValidator = searchValidator;
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

    /// <summary>
    /// Updates an existing flight
    /// </summary>
    /// <param name="id">Flight ID</param>
    /// <param name="updateFlightDto">Flight update data</param>
    /// <returns>Updated flight</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(FlightDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlightDto>> UpdateFlight(int id, [FromBody] UpdateFlightDto updateFlightDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateFlightDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedFlight = await _flightService.UpdateFlightAsync(id, updateFlightDto);

        if (updatedFlight == null)
        {
            return NotFound($"Flight with ID {id} not found.");
        }

        return Ok(updatedFlight);
    }

    /// <summary>
    /// Deletes a flight
    /// </summary>
    /// <param name="id">Flight ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFlight(int id)
    {
        var result = await _flightService.DeleteFlightAsync(id);

        if (!result)
        {
            return NotFound($"Flight with ID {id} not found.");
        }

        return NoContent();
    }

    /// <summary>
    /// Searches flights by various criteria
    /// </summary>
    /// <param name="searchDto">Search criteria</param>
    /// <returns>List of matching flights</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<FlightDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<FlightDto>>> SearchFlights([FromQuery] FlightSearchDto searchDto)
    {
        var validationResult = await _searchValidator.ValidateAsync(searchDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var flights = await _flightService.SearchFlightsAsync(searchDto);
        return Ok(flights);
    }
}