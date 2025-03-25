using EMS.Application.Dtos;
using EMS.Domain.Enums;

namespace EMS.Application.Interfaces;

public interface IEventWriteService
{
    Task<EventDto?> CreateEventAsync(EventCrudDto eventCrudDto, CancellationToken cancellationToken);
    Task<EventDto?> ModifyEventAsync(EventCrudDto eventCrudDto, int EventId, CancellationToken cancellationToken);
    Task<EventDto?> DeleteEventAsync(int EventId, CancellationToken cancellationToken);
    Task<SignUpResult> SignUpForEventAsync(int eventId, string userId, CancellationToken cancellationToken);

}
