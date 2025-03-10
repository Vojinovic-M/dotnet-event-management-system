using EMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace EMS.Web.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController(IEventReadService eventReadService) : ControllerBase
{
    private readonly IEventReadService _eventReadService = eventReadService;


    [HttpGet]
    public async Task<IActionResult> GetAllEvents(CancellationToken cancellationToken)
    {
        var eventDtos = await _eventReadService.GetAllEventsAsync(cancellationToken);

        if (eventDtos == null)
            return NotFound();

        return Ok(eventDtos);
    }

    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetEventById(int eventId, CancellationToken cancellationToken)
    {
        var eventDto = await _eventReadService.GetEventByIdAsync(eventId, cancellationToken);

        if (eventDto == null)
            return NotFound();

        return Ok(eventDto);
    }

}
