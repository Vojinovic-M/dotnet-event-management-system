using EMS.Application.Dtos;
using EMS.Application.Interfaces;
using MediatR;

namespace EMS.Application.Features.Events.Queries.GetEventById;

class GetEventByIdHandler(IEventReadService eventReadService) : IRequestHandler<GetEventByIdQuery, EventDto>
{
    private readonly IEventReadService _eventReadService = eventReadService;

    public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        return await _eventReadService.GetEventByIdAsync(request.EventId, cancellationToken);
    }
}
