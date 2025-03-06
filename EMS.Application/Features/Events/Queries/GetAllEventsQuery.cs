using MediatR;
using EMS.Application.Features.Events.Dtos;

namespace EMS.Application.Features.Events.Queries;
class GetAllEventsQuery : IRequest<List<EventDto>>
{
}
