using Application.Common.Dtos.Order;
using Application.UseCases.OrderCases.Commands.CreateOrderCase;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles.OrderProfiles;

public class CreateOrderMappingProfile : Profile
{
    public CreateOrderMappingProfile()
    {
        CreateMap<CreateOrderCommand, Order>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.DeliveryDate, opt => opt.MapFrom(src => src.DeliveryDate))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

        CreateMap<CreateOrderDto, CreateOrderCommand>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.DeliveryDate, opt => opt.MapFrom(src => src.DeliveryDate))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
    }
}