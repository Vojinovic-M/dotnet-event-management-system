using EMS.Application.Dtos;

namespace EMS.Application.Interfaces;

public interface IEventReadService
{
    Task<PaginatedList<EventDto>> GetEventsAsync(EventPaginationRequest request, CancellationToken cancellationToken);
    Task<EventDto?> GetEventByIdAsync(int EventId, CancellationToken cancellationToken);
    Task<IEnumerable<EventDto>> GetUserEventsAsync(string userId, CancellationToken cancellationToken);
    Task<IEnumerable<EventDto>> GetSignedUpEventsAsync(string userId, CancellationToken cancellationToken);
    Task<IEnumerable<ReviewDto>> GetReviewsAsync(int eventId, CancellationToken cancellationToken);
}
