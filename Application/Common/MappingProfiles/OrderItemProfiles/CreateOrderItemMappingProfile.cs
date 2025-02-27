using Application.Common.Dtos.OrderItem;
using Application.UseCases.OrderItemCases.Commands.CreateOrderItemCase;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles.OrderItemProfiles;

public class CreateOrderItemMappingProfile : Profile
{
    public CreateOrderItemMappingProfile()
    {
        CreateMap<CreateOrderItemDto, CreateOrderItemCommand>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
        
        CreateMap<CreateOrderItemCommand, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid())) 
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
    }
}