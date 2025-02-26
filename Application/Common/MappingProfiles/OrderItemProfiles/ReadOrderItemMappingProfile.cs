using Application.Common.Dtos.OrderItem;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles.OrderItemProfiles;

public class ReadOrderItemMappingProfile : Profile
{
    public ReadOrderItemMappingProfile()
    {
        CreateMap<OrderItem, ReadOrderItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
    }
}