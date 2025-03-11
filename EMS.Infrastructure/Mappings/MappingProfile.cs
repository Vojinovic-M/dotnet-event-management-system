using EMS.Application.Dtos;
using EMS.Domain.Entities;
using AutoMapper;
using EMS.Domain.Enums;

namespace EMS.Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Event, EventDto>()
            .ForMember(dest => dest.Category,
                opt => opt.MapFrom(src => src.Category.ToString()))
            .ReverseMap() // od EventDto na Event
            .ForMember(dest => dest.Category,
                opt => opt.MapFrom(src => ParseCategory(src.Category)));

        RecognizeDestinationPrefixes("Dto"); // Matches properties like "DtoProperty"
        SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
        DestinationMemberNamingConvention = PascalCaseNamingConvention.Instance;
    }

    private static EventCategory ParseCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category)) throw new AutoMapperMappingException("Category cannot be empty");

        if (Enum.TryParse<EventCategory>(category, ignoreCase: true, out var parseCategory))
        {
            return parseCategory;
        }
        throw new AutoMapperMappingException($"Invalid category: {category}");
    }

}
