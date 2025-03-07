using EMS.Application.Dtos;
using EMS.Application.Interfaces;
using MediatR;

namespace EMS.Application.Features.Events.Queries.GetAllEvents;

class GetAllEventsHandler(IEventReadService eventReadService) : IRequestHandler<GetAllEventsQuery, List<EventDto>>
{
    private readonly IEventReadService _eventReadService = eventReadService;

    public async Task<List<EventDto>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
    {
        return await _eventReadService.GetAllEventsAsync(cancellationToken);
    }
}


