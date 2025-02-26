using Application.Common.Dtos.Order;
using Application.Common.MappingProfiles.OrderItemProfiles;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles.OrderProfiles;

public class ReadOrderMappingProfile : Profile
{
    public ReadOrderMappingProfile()
    {
        CreateMap<Order, ReadOrderDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.DeliveryDate, opt => opt.MapFrom(src => src.DeliveryDate));
    }
}