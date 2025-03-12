using AutoMapper;
using EMS.Application.Dtos;
using EMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Web.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles= "Admin")]
public class EventsAdminController(
    IEventWriteService eventWriteService, ILogger<EventsAdminController> logger) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)  return BadRequest(ModelState);
            
            var createdEvent = await eventWriteService.CreateEventAsync(eventDto, cancellationToken);

            return Created($"/api/events/{createdEvent.EventId}", createdEvent );

        } catch (AutoMapperMappingException ex)
        {
            logger.LogError(ex, "Invalid category value");
            return BadRequest(ex.Message);
        }
    }


}
