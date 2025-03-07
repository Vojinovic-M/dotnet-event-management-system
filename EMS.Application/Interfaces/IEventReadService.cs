using EMS.Application.Dtos;

namespace EMS.Application.Interfaces;

public interface IEventReadService
{
    Task<List<EventDto>> GetAllEventsAsync(CancellationToken cancellationToken);
    Task<EventDto> GetEventByIdAsync(int EventId, CancellationToken cancellationToken);
}
