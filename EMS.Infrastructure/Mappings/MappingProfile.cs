﻿using EMS.Application.Dtos;
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
                opt => opt.MapFrom(src => ParseCategory(src.Category ?? "Meeting")));
        
        
        CreateMap<Event, EventCrudDto>()
            .ForMember(dest => dest.Category,
                opt => opt.MapFrom(src => src.Category.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Image, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Category) ? ParseCategory(src.Category) : EventCategory.Meeting));

        CreateMap<CreateEventDto, Event>()
            .ForMember(dest => dest.Image, opt => opt.Ignore());
    }


    private static EventCategory ParseCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category)) 
            throw new AutoMapperMappingException("Category cannot be empty");

        if (Enum.TryParse<EventCategory>(category, ignoreCase: true, out var parseCategory))
        {
            return parseCategory;
        }
        throw new AutoMapperMappingException($"Invalid category: {category}");
    }

}
