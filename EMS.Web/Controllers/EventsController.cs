using EMS.Application.Dtos;
using EMS.Application.Interfaces;
using EMS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EMS.Web.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController(IEventReadService eventReadService, IEventWriteService eventWriteService) : ControllerBase
{
    private readonly IEventReadService _eventReadService = eventReadService;
    private readonly IEventWriteService _eventWriteService = eventWriteService;

    [HttpGet]
    public async Task<IActionResult> GetEvents([FromQuery] EventPaginationRequest request, CancellationToken cancellationToken)
    {
        var paginatedEvents = await _eventReadService.GetEventsAsync(request, cancellationToken);
        return Ok(paginatedEvents);
    }


    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetEventById(int eventId, CancellationToken cancellationToken)
    {
        var eventDto = await _eventReadService.GetEventByIdAsync(eventId, cancellationToken);

        if (eventDto == null)  return NotFound();

        return Ok(eventDto);
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserEvents(string userId, CancellationToken cancellationToken)
    {
        var isUser = User.IsInRole("User");
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && !isUser)    {  return Forbid();  }

        var events = await _eventReadService.GetUserEventsAsync(userId, cancellationToken);
        return Ok(events);
    }


    [HttpPost("signup/{eventId}")]
    [Authorize]
    public async Task<IActionResult> SignUpForEvent(int eventId, [FromBody] string userId, CancellationToken cancellationToken)
    {
        var result = await _eventWriteService.SignUpForEventAsync(eventId, userId, cancellationToken);

        return result switch
        {
            SignUpResult.Success => Ok(new { success = true, message = "User signed up successfully." }),

            SignUpResult.EventNotFound => Ok(new { success = false, message = "The event does not exist." }),

            SignUpResult.AlreadySignedUp => Ok(new { success = false, message = "User is already signed up for the event." }),

            _ => StatusCode(500, new { success = false, message = "An unexpected error occurred." }) // fallback
        };
    }


    [HttpGet("signedup/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetSignedUpEvents(string userId, CancellationToken cancellationToken)
    {
        var events = await _eventReadService.GetSignedUpEventsAsync(userId, cancellationToken);
        
        return Ok(events);
    }
}
