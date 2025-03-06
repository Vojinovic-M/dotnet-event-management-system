using EMS.Application.Interfaces;
using EMS.Application.Dtos;
using EMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EMS.Infrastructure.Services;

public class EventReadService : IEventReadService
{
    private readonly ApplicationDbContext _context;

    public EventReadService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<EventDto>> GetAllEventsAsync(CancellationToken cancellationToken)
    {
        return await _context.Events
            .Select(e => new EventDto
            {
                Id = e.Id,
                Name = e.Name,
                Date = e.Date,
                Location = e.Location,
                Description = e.Description,
                ImageUrl = e.ImageUrl
            })
            .ToListAsync(cancellationToken);
    }
}
