using MediatR;
using EMS.Application.Dtos;

namespace EMS.Application.Features.Events.Queries.GetEventById;

class GetEventByIdQuery(int eventId) : IRequest<EventDto>
{
    public int EventId { get; set; } = eventId;
}
