using Application.Common.Dtos.OrderItem;
using Application.UseCases.OrderItemCases.Commands.UpdateOrderItemCase;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles.OrderItemProfiles;

public class UpdateOrderItemMappingProfile : Profile
{
    public UpdateOrderItemMappingProfile()
    {
        CreateMap<UpdateOrderItemDto, UpdateOrderItemCommand>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
        
        CreateMap<UpdateOrderItemCommand, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
    }
}