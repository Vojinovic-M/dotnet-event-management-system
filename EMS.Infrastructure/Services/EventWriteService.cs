using AutoMapper;
using EMS.Application.Dtos;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Domain.Enums;
using EMS.Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EMS.Infrastructure.Services;

public class EventWriteService(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : IEventWriteService
{

    private readonly ApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;


    private async Task<string> SaveImage(IFormFile image)
    {
        var request = _httpContextAccessor.HttpContext.Request;

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        if (!Directory.Exists(uploadsFolder))   Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
            await image.CopyToAsync(fileStream);

        return $"{request.Scheme}://{request.Host}/uploads/{uniqueFileName}";
    }



    public async Task<EventDto?> CreateEventAsync(CreateEventDto createEventDto, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var image = await SaveImage(createEventDto.Image);

        var newEvent = _mapper.Map<Event>(createEventDto);
        newEvent.Image = image;

        await _context.Events.AddAsync(newEvent, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var eventOwner = new EventOwner
        {
            EventId = newEvent.EventId,
            UserId = userId,
        };

        await _context.EventOwners.AddAsync(eventOwner, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventDto>(newEvent);
    }


    public async Task<EventDto?> ModifyEventAsync(EventCrudDto eventCrudDto, int eventId, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext.User;
        var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken);

        if (eventCrudDto.Image != null) existingEvent.Image = await SaveImage(eventCrudDto.Image);

        existingEvent.Name = eventCrudDto.Name ?? existingEvent.Name;
        existingEvent.Date = eventCrudDto.Date;
        existingEvent.Location = eventCrudDto.Location ?? existingEvent.Location;
        existingEvent.Description = eventCrudDto.Description ?? existingEvent.Description;
        existingEvent.Category = Enum.Parse<EventCategory>(eventCrudDto.Category);

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = user.IsInRole("Admin");
        var isOwner = await _context.EventOwners.AnyAsync(eo => eo.EventId == eventId && eo.UserId == userId, cancellationToken);

        if (!isOwner && !isAdmin) { return null; }

        _mapper.Map(eventCrudDto, existingEvent);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventDto>(existingEvent);
    }


    public async Task<EventDto?> DeleteEventAsync(int eventId, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) {  return null;  }
        
        var eventDelete = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken);
        if (eventDelete == null) {  return null;  }

        var isAdmin = user.IsInRole("Admin");
        var isOwner = await _context.EventOwners.AnyAsync(eo => eo.EventId == eventId && eo.UserId == userId, cancellationToken);
        if (!isOwner && !isAdmin) { return null; }

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
