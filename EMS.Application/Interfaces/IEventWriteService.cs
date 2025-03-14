using EMS.Application.Dtos;

namespace EMS.Application.Interfaces;

public interface IEventWriteService
{
    Task<EventDto> CreateEventAsync(EventDto eventDto, CancellationToken cancellationToken);
    Task<EventDto?> ModifyEventAsync(EventDto eventDto, int EventId, CancellationToken cancellationToken);
    Task<EventDto?> DeleteEventAsync(int EventId, CancellationToken cancellationToken);

}
