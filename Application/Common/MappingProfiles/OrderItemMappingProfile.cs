using Application.Common.Dtos.OrderItem;
using Application.UseCases.OrderItemCases.Commands.CreateOrderItemCase;
using Application.UseCases.OrderItemCases.Commands.DeleteOrderItemCase;
using Application.UseCases.OrderItemCases.Commands.UpdateOrderItemCase;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles;

public class OrderItemMappingProfile : Profile
{
    public OrderItemMappingProfile()
    {
        CreateMap<OrderItem, ReadOrderItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
        
        CreateMap<CreateOrderItemDto, CreateOrderItemCommand>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
        
        CreateMap<CreateOrderItemCommand, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid())) 
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
        
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
        
        CreateMap<DeleteOrderItemDto, DeleteOrderItemCommand>()
            .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.Id));
    }
}