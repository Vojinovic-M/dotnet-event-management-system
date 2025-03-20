using EMS.Application.Dtos;
using EMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

}
