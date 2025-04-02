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
    public async Task<IActionResult> CreateEvent( 
        [FromForm] CreateEventDto createEventDto,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)  
                return BadRequest(ModelState);

            if (createEventDto.Image == null || createEventDto.Image.Length == 0)
                return BadRequest("Image file is required");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(createEventDto.Image.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type. Allowed types: JPG, JPEG, PNG");

            if (createEventDto.Image.Length > 5 * 1024 * 1024) // 5 MB
                return BadRequest("File size exceeds 5MB limit");

            var createdEvent = await eventWriteService.CreateEventAsync(createEventDto, cancellationToken);
            return Created($"/api/events/{createdEvent?.EventId}", createdEvent );

        } 
        catch (AutoMapperMappingException ex)
        {
            logger.LogError(ex, "Invalid category value");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating event");
            return StatusCode(500, "Intenal server error");
        }
    } // UMESTO IF VALIDACIJA: STATUSCODES.(KOD) I CUSTOM ATRIBUTI PREKO ANOTACIJA


    [HttpPut("modify/{eventId}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ModifyEvent( [FromForm] EventCrudDto eventCrudDto, [FromRoute] int eventId, CancellationToken cancellationToken)
    {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var modifiedEvent = await eventWriteService.ModifyEventAsync(eventCrudDto, eventId, cancellationToken);
            if (modifiedEvent == null) return NotFound();

            return Ok(modifiedEvent);
    }


    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteEvent(int id, CancellationToken cancellationToken)
    {
        var deletedEvent = await eventWriteService.DeleteEventAsync(id, cancellationToken);
        if (deletedEvent == null) return NotFound();

        return Ok(deletedEvent);

    }

    [HttpPost("{eventId}/reviews")]
    public async Task<IActionResult> AddReview([FromBody] ReviewRequestDto reviewRequest, CancellationToken cancellationToken)
    {
        var review = await eventWriteService.AddReviewAsync(reviewRequest, cancellationToken);
    
        if (review == null) return NotFound("Event not found");

        return Ok(review);
    }

    [HttpGet("reviews/{eventId}")]
    public async Task<IActionResult> GetReviews([FromRoute] int eventId, CancellationToken cancellationToken)
    {
        var reviews = await eventReadService.GetReviewsAsync(eventId, cancellationToken);
        return Ok(reviews);
    }
}
