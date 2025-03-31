using EMS.Application.Dtos;
using EMS.Domain.Entities;
using EMS.Domain.Enums;

namespace EMS.Application.Interfaces;

public interface IEventWriteService
{
    Task<EventDto?> CreateEventAsync(CreateEventDto createEventDto, CancellationToken cancellationToken);
    Task<EventDto?> ModifyEventAsync(EventCrudDto eventCrudDto, int EventId, CancellationToken cancellationToken);
    Task<EventDto?> DeleteEventAsync(int EventId, CancellationToken cancellationToken);
    Task<SignUpResult> SignUpForEventAsync(int eventId, string userId, CancellationToken cancellationToken);
    Task<EventReview?> AddReviewAsync(ReviewRequestDto reviewRequest, CancellationToken cancellationToken);

}
