using AutoMapper;
using EMS.Application.Dtos;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EMS.Infrastructure.Services;

public class EventWriteService(ApplicationDbContext context, IMapper mapper) : IEventWriteService
{

    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));


    public async Task<EventDto> CreateEventAsync(EventDto eventDto, CancellationToken cancellationToken)
    {
        var newEvent = _mapper.Map<Event>(eventDto);

        await _context.Events.AddAsync(newEvent, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventDto>(newEvent);
    }


    public async Task<Event?> ModifyEventAsync(EventCrudDto eventCrudDto, int EventId, CancellationToken cancellationToken)
    {
        var existingEvent = await _context.Events
            .FirstOrDefaultAsync(e => e.EventId == EventId, cancellationToken);

        if (existingEvent == null) { return null; }

        _mapper.Map(eventCrudDto, existingEvent);

        await _context.SaveChangesAsync(cancellationToken);
        return existingEvent;
    }


    public async Task<EventDto?> DeleteEventAsync(int EventId, CancellationToken cancellationToken)
    {
        var eventDelete = await _context.Events
            .FirstOrDefaultAsync(e => e.EventId == EventId, cancellationToken);

        if (eventDelete == null) { return null; }

        _context.Events.Remove(eventDelete);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EventDto>(eventDelete);
    }
}
