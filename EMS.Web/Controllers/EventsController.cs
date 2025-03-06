using EMS.Application.Interfaces;
using EMS.Application.Features.Events.Queries.GetAllEvents;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace EMS.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(IMediator mediator, IEventReadService eventReadService) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IEventReadService _eventReadService = eventReadService;


    [HttpGet]
    public async Task<IActionResult> GetAllEvents(CancellationToken cancellationToken)
    {
        var events = await _mediator.Send(new GetAllEventsQuery(), cancellationToken);
        return Ok(events);
    }

}
