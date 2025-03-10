using EMS.Application.Interfaces;
using EMS.Application.Dtos;
using EMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EMS.Infrastructure.Services;

public class EventReadService(ApplicationDbContext context) : IEventReadService
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<List<EventDto>> GetAllEventsAsync(CancellationToken cancellationToken)
    {
        return await _context.Events
            .Select(e => new EventDto
            {
                EventId = e.EventId,
                Name = e.Name,
                Date = e.Date,
                Location = e.Location,
                Description = e.Description,
                ImageUrl = e.ImageUrl
            })
            .ToListAsync(cancellationToken);
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
                ImageUrl = e.ImageUrl,
                Category = e.Category.ToString()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
