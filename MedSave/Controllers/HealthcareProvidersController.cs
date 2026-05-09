using MedSave.DTOs.HealthcareProviders;
using MedSave.Services.HealthcareProviders;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace MedSave.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthcareProvidersController : ControllerBase
{
    private readonly IHealthcareProvidersService _healthcareProvidersService;

    public HealthcareProvidersController(IHealthcareProvidersService healthcareProvidersService)
    {
        _healthcareProvidersService = healthcareProvidersService;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var dtos = await _healthcareProvidersService.GetAllAsync();

            return Ok(new { items = dtos });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Internal error when searching for the Healthcare Providers",
                details = ex.Message
            });
        }
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var dto = await _healthcareProvidersService.GetByIdAsync(id);

            return Ok(dto);
        }
        catch (HealthcareProvidersService.NotFoundException ex)
        {
            return StatusCode(404, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Internal error when searching for the Healthcare Provider",
                details = ex.Message
            });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddHealthcareProviders([FromBody] CreateHealthcareProviderRequest req)
    {
        try
        {
            var created = await _healthcareProvidersService.AddAsync(req.HealthcareProvidersDto, req.AddressStockDto,
                req.ProviderTypeDto);

            return CreatedAtAction(nameof(GetById), new { id = created.HealthcareProviderId }, created);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = "Invalid request body." });
        }
        catch (HealthcareProvidersService.ConflictException ex)
        {
            return StatusCode(409, new { message = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Internal error when adding the Healthcare Provider",
                details = ex.Message
            });
        }
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateHealthcareProvider(long id, HealthcareProvidersDTO healthcareProviderDto)
    {
        try
        {
            await _healthcareProvidersService.UpdateAsync(id, healthcareProviderDto);

            return Ok(new {message = $"Healthcare Provider with {id} updated"});
        }
        catch (HealthcareProvidersService.NotFoundException ex)
        {
            return NotFound(new { message = $"Healthcare Provider with id {id} not found" });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = "Invalid request body." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = $"Internal error when updating the Healthcare Provider",
                details = ex.Message
            });
        }
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteHealthcareProvider(long id)
    {
        try
        {
            await _healthcareProvidersService.DeleteAsync(id);

            return Ok(new { message = $"Healthcare Provider with {id} deleted" });
        }
        catch (HealthcareProvidersService.NotFoundException ex)
        {
            return NotFound(new { message = $"Healthcare Provider with {id} not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = $"Internal error when deleting the Healthcare Provider",
                details = ex.Message
            });
        }
    }
}