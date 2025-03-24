using EMS.Application.Dtos;
using EMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace EMS.Web.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController(IEventReadService eventReadService) : ControllerBase
{
    private readonly IEventReadService _eventReadService = eventReadService;

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

}
