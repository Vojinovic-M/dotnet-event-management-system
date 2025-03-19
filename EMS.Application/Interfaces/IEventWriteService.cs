using EMS.Application.Dtos;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces;

public interface IEventWriteService
{
    Task<EventDto> CreateEventAsync(EventDto eventDto, CancellationToken cancellationToken);
    Task<Event?> ModifyEventAsync(EventCrudDto eventCrudDto, int EventId, CancellationToken cancellationToken);
    Task<EventDto?> DeleteEventAsync(int EventId, CancellationToken cancellationToken);

}
