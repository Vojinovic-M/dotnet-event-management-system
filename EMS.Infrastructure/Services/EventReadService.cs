using EMS.Application.Interfaces;
using EMS.Application.Dtos;
using EMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Enums;

namespace EMS.Infrastructure.Services;

public class EventReadService(ApplicationDbContext context) : IEventReadService
{
    private readonly ApplicationDbContext _context = context;


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

        if (request.UpcomingOnly) query
                = query.Where(e => e.Date.Date >= DateTime.Today);

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
        var userEvents = await _context.EventOwners
            .Where(e => e.UserId == userId)
            .Select(e => e.Event)
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

        var signedUpEvents = await _context.EventRegistrations
            .Where(e => e.UserId == userId)
            .Select(e => e.Event)
            .ToListAsync(cancellationToken);

        return signedUpEvents.Select(e => new EventDto
        {
            EventId = e.EventId,
            Name = e.Name,
            Date = e.Date,
            Location = e.Location,
            Description = e.Description,
            Image = e.Image,
            Category = e.Category.ToString(),
        });
    }

    public async Task<PaginatedList<EventDto>> GetEventsWithReviewsAsync(EventPaginationRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Events.AsQueryable();

        var events = await query
            .Include(e => e.EventReviews) // Eager load reviews
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var eventDtos = events.Select(e => new EventDto
        {
            EventId = e.EventId,
            Name = e.Name,
            Date = e.Date,
            Location = e.Location,
            Description = e.Description,
            Image = e.Image,
            Category = e.Category.ToString(),
            AverageRating = e.EventReviews.Any() ? e.EventReviews.Average(er => er.RatingStars) : 0, // Calculate average rating
            ReviewsCount = e.EventReviews.Count
        }).ToList();

        return new PaginatedList<EventDto>
        {
            Items = eventDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = events.Count
        };
    }
}
