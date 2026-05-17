using MedSave.DTOs;
using MedSave.DTOs.UsersSys;
using MedSave.Services.UsersSys;
using Microsoft.AspNetCore.Mvc;

namespace MedSave.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersSysController : ControllerBase
{
    private readonly IUsersSysService _usersSysService;

    public UsersSysController(IUsersSysService usersSysService)
    {
        _usersSysService = usersSysService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var dtos = await _usersSysService.GetAllAsync();

            return Ok(new { items = dtos });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                message = "Internal error when searching for the Users",
                details = e.Message
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
            var dto = await _usersSysService.GetByIdAsync(id);

            return Ok(dto);
        }
        catch (UsersSysService.NotFoundException ex)
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddUsersSys([FromBody] CreateUserRequest createUserRequest)
    {
        try
        {
            var created = await _usersSysService.AddAsync(createUserRequest);

            return CreatedAtAction(nameof(GetById), new { id = created.UserId }, created);
        }
        catch (UsersSysService.ConflictException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = "Invalid request body." });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                message = "Internal error when searching for the Healthcare Provider",
                details = e.Message
            });
        }
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser(long id, [FromBody] UsersSysDTO dto)
    {
        try
        {
            await _usersSysService.UpdateAsync(id, dto);

            return Ok(new { message = $"User with id {id} updated" });
        }
        catch (UsersSysService.NotFoundException ex)
        {
            return NotFound(new { message = $"User with Id {id} not found" });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = "Invalid request body." });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                message = "Internal error when updating the manufacturer",
                details = e.Message
            });
        }
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(long id)
    {
        try
        {
            await _usersSysService.DeleteAsync(id);

            return Ok(new { message = $"User with id {id} deleted" });
        }
        catch (UsersSysService.NotFoundException ex)
        {
            return NotFound(new { message = $"User with id {id} not found" });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                message = $"Internal error when deleting the Healthcare Provider",
                details = e.Message
            });
        }
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Search(
        [FromQuery] string? nameUser,
        [FromQuery] string? email,
        [FromQuery] long? roleUserId,
        [FromQuery] long? profUserId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "userId",
        [FromQuery] string sortDir = "asc")
    {
        try
        {
            var result = await _usersSysService.SearchAsync(nameUser, email, roleUserId, profUserId, page, pageSize, sortBy, sortDir);

            return Ok(new { Items = result.Items, PageInfo = result.PageInfo });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Internal error when searching the manufacturer",
                details = ex.Message
            });
        }
    }
}