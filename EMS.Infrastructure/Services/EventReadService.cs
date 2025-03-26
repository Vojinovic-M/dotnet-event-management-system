using EMS.Application.Interfaces;
using EMS.Application.Dtos;
using EMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Enums;
using AutoMapper;

namespace EMS.Infrastructure.Services;

public class EventReadService(ApplicationDbContext context, IMapper mapper) : IEventReadService
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));


    public async Task<PaginatedList<EventDto>> GetEventsAsync(EventPaginationRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Events.AsQueryable(); // pravi objekat za query

        if (request.EventDate.HasValue)
            query = query.Where(e => e.Date.Date == request.EventDate.Value.Date); // filter da bude vrednost kao u request

        if (!string.IsNullOrEmpty(request.Category))
        {
            if (!Enum.TryParse<EventCategory>(request.Category, true, out var category))
                throw new ArgumentException("Invalid category");

            query = query.Where(e => e.Category == category); 
        }

        query = request.SortBy.ToLower() switch
        {
            "name" => request.SortOrder == "asc"
                ? query.OrderBy(e => e.Name)
                : query.OrderByDescending(e => e.Name),
            _ => request.SortOrder == "asc" // default sortiranje je po datumu ascending
                ? query.OrderBy(e => e.Date)
                : query.OrderByDescending(e => e.Date)
        };

        // paginacija
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var eventDtos = items.Select(e => new EventDto
        {
            EventId = e.EventId,
            Name = e.Name,
            Date = e.Date,
            Location = e.Location,
            Description = e.Description,
            Image = e.Image,
            Category = e.Category.ToString()
        }).ToList();

        return new PaginatedList<EventDto>
        {
            Items = eventDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }


    public async Task<IEnumerable<EventDto>> GetUserEventsAsync(string userId, CancellationToken cancellationToken)
    {
        var userEvents = await _context.Events
            .Where(e => e.UserId == userId)
            .ToListAsync(cancellationToken);

        return userEvents.Select(e => new EventDto
        {
            EventId = e.EventId,
            Name = e.Name,
            Date = e.Date,
            Location = e.Location,
            Description = e.Description,
            Image = e.Image,
            Category = e.Category.ToString(),
            UserId = e.UserId,
            UsersInEvent = e.UsersInEvent
        });
    }


    public async Task<EventDto?> GetEventByIdAsync(int eventId, CancellationToken cancellationToken)
    {
        return await _context.Events
            .Select(e => new EventDto
            {
                EventId = e.EventId,
                Name = e.Name,
                Date = e.Date,
                Location = e.Location,
                Description = e.Description,
                Image = e.Image,
                Category = e.Category.ToString()
            })
            .FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken);
    }


    public async Task<IEnumerable<EventDto>> GetSignedUpEventsAsync(string userId, CancellationToken cancellationToken)
    {

        var events = await _context.Events
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var signedUpEvents = events
            .Where(e => e.UsersInEvent != null && e.UsersInEvent.Contains(userId))
            .ToList();

        return signedUpEvents.Select(e => new EventDto
        {
            EventId = e.EventId,
            Name = e.Name,
            Date = e.Date,
            Location = e.Location,
            Description = e.Description,
            Image = e.Image,
            Category = e.Category.ToString(),
            UserId = e.UserId,
            UsersInEvent = e.UsersInEvent
        });
    }

}
