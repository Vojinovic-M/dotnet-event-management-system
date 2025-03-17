﻿using AutoMapper;
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

    [HttpPost("modify")]
    public async Task<IActionResult> ModifyEvent([FromBody] EventDto eventDto, int EventId, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var modifiedEvent = await eventWriteService.ModifyEventAsync(eventDto, EventId, cancellationToken);
            if (modifiedEvent == null) return NotFound();

            return Ok(modifiedEvent);
        }
        catch (AutoMapperMappingException ex)
        {
            logger.LogError(ex, "Invalid category value");
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteEvent(int id, CancellationToken cancellationToken)
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
}
