using EMS.Application.Interfaces;
using EMS.Application.Features.Events.Queries.GetAllEvents;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace EMS.Web.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController(IMediator mediator, IEventReadService eventReadService) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IEventReadService _eventReadService = eventReadService;


    [HttpGet]
    public async Task<IActionResult> GetAllEvents(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetAllEventsQuery(), cancellationToken));

    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetEventById(int eventId, CancellationToken cancellationToken)
    {
        var eventDto = await _eventReadService.GetEventByIdAsync(eventId, cancellationToken);

        if (eventDto == null)
            return NotFound();

        return Ok(eventDto);
    }

}
