using AutoMapper;
using EMS.Application.Dtos;
using EMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Web.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
public class EventsAdminController(
    IEventWriteService eventWriteService, 
    IEventReadService eventReadService,
    ILogger<EventsAdminController> logger) : ControllerBase
{

    [HttpPost("create")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateEvent( [FromForm] CreateEventDto createEventDto, CancellationToken cancellationToken)
    {
            if (!ModelState.IsValid)  return BadRequest(ModelState);
            
            var createdEvent = await eventWriteService.CreateEventAsync(createEventDto, cancellationToken);
            return Created($"/api/events/{createdEvent?.EventId}", createdEvent );
    }


    [HttpPut("modify/{eventId}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ModifyEvent( [FromForm] EventCrudDto eventCrudDto, [FromRoute] int eventId, CancellationToken cancellationToken)
    {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var modifiedEvent = await eventWriteService.ModifyEventAsync(eventCrudDto, eventId, cancellationToken);
            return Ok(modifiedEvent);
    }


    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteEvent(int id, CancellationToken cancellationToken)
    {
        var deletedEvent = await eventWriteService.DeleteEventAsync(id, cancellationToken);
        return Ok(deletedEvent);

    }

    [HttpPost("{eventId}/reviews")]
    public async Task<IActionResult> AddReview([FromBody] ReviewRequestDto reviewRequest, CancellationToken cancellationToken)
    {
        var review = await eventWriteService.AddReviewAsync(reviewRequest, cancellationToken);
        return Ok(review);
    }

    [HttpGet("reviews/{eventId}")]
    public async Task<IActionResult> GetReviews([FromRoute] int eventId, CancellationToken cancellationToken)
    {
        var reviews = await eventReadService.GetReviewsAsync(eventId, cancellationToken);
        return Ok(reviews);
    }
}
