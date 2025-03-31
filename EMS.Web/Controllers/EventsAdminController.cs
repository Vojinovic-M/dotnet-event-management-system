using AutoMapper;
using EMS.Application.Dtos;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Web.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
public class EventsAdminController(
    IEventWriteService eventWriteService, ILogger<EventsAdminController> logger) : ControllerBase
{

    [HttpPost("create")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateEvent(
        [FromForm] CreateEventDto createEventDto,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)  return BadRequest(ModelState);

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
    }


    [HttpPut("modify/{eventId}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ModifyEvent(
        [FromForm] EventCrudDto eventCrudDto,
        [FromRoute] int eventId,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var modifiedEvent = await eventWriteService.ModifyEventAsync(eventCrudDto, eventId, cancellationToken);
            if (modifiedEvent == null) return NotFound();

            return Ok(modifiedEvent);
        }
        catch (AutoMapperMappingException ex)
        {
            logger.LogError(ex, "Invalid category value");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error modifying event");
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteEvent(
        int id, 
        CancellationToken cancellationToken)
    {
        try
        {
            var deletedEvent = await eventWriteService.DeleteEventAsync(id, cancellationToken);
            if (deletedEvent == null) return NotFound();

            return Ok(deletedEvent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting event");
            return StatusCode(500, "Internal Server error");
        }
    }

    [HttpPost("{eventId}/reviews")]
    public async Task<IActionResult> AddReview([FromBody] ReviewRequestDto reviewRequest, CancellationToken cancellationToken)
    {
        var review = await eventWriteService.AddReviewAsync(reviewRequest, cancellationToken);
    
        if (review == null) return NotFound("Event not found");

        return Ok(review);
    }
}
