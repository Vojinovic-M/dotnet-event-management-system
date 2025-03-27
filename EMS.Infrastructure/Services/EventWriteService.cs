using AutoMapper;
using EMS.Application.Dtos;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Domain.Enums;
using EMS.Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EMS.Infrastructure.Services;

public class EventWriteService(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : IEventWriteService
{

    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public async Task<EventDto?> CreateEventAsync(EventCrudDto eventCrudDto, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var newEvent = _mapper.Map<Event>(eventCrudDto);
        newEvent.UserId = userId;

        await _context.Events.AddAsync(newEvent, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventDto>(newEvent);
    }


    public async Task<EventDto?> ModifyEventAsync(EventCrudDto eventCrudDto, int EventId, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == EventId, cancellationToken);
        if (existingEvent == null) { return null; }

        var isAdmin = user.IsInRole("Admin");
        if (existingEvent.UserId != userId && !isAdmin) {  return null;  }

        _mapper.Map(eventCrudDto, existingEvent);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventDto>(existingEvent);
    }


    public async Task<EventDto?> DeleteEventAsync(int EventId, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) {  return null;  }
        
        var eventDelete = await _context.Events.FirstOrDefaultAsync(e => e.EventId == EventId, cancellationToken);
        if (eventDelete == null) {  return null;  }

        var isAdmin = user.IsInRole("Admin");
        if (eventDelete.UserId != userId && !isAdmin) { return null; }

        _context.Events.Remove(eventDelete);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventDto>(eventDelete);
    }



    public async Task<SignUpResult> SignUpForEventAsync(int eventId, string userId, CancellationToken cancellationToken)
    {
        var eventEntity = await _context.Events
            .FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken);

        if (eventEntity == null) return SignUpResult.EventNotFound;

        var existingRegistration = await _context.EventRegistrations
            .FirstOrDefaultAsync(er => er.EventId == eventId && er.UserId == userId, cancellationToken);

        if (existingRegistration != null) return SignUpResult.AlreadySignedUp;

        var eventRegistration = new EventRegistration
        {
            EventId = eventId,
            UserId = userId
        };

        await _context.EventRegistrations.AddAsync(eventRegistration, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return SignUpResult.Success;
    }




}
