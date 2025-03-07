using MediatR;
using EMS.Application.Dtos;

namespace EMS.Application.Features.Events.Queries.GetAllEvents;

public class GetAllEventsQuery : IRequest<List<EventDto>>
{
}
