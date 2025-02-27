using Application.Common.Dtos.Order;
using Application.UseCases.OrderCases.Queries.GetOrdersByFilterCase;
using AutoMapper;

namespace Application.Common.MappingProfiles.OrderProfiles;

public class FilterOrderMappingProfile : Profile
{
    public FilterOrderMappingProfile()
    {
        CreateMap<GetOrderByFilterDto, GetOrdersByFilterQuery>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.DeliveryDate, opt => opt.MapFrom(src => src.DeliveryDate))
            .ForMember(dest => dest.PageInfoDto, opt => opt.MapFrom(src => src.PageInfoDto));
    }
}