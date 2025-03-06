using EMS.Application.Dtos;

namespace EMS.Application.Interfaces;

public interface IEventReadService
{
    Task<List<EventDto>> GetAllEventsAsync(CancellationToken cancellationToken);
}
