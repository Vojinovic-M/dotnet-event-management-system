using EMS.Application.Dtos;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces;

public interface IEventWriteService
{
    Task<EventDto?> CreateEventAsync(EventCrudDto eventCrudDto, CancellationToken cancellationToken);
    Task<EventDto?> ModifyEventAsync(EventCrudDto eventCrudDto, int EventId, CancellationToken cancellationToken);
    Task<EventDto?> DeleteEventAsync(int EventId, CancellationToken cancellationToken);

}
