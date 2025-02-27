using Application.Common.Dtos.Order;
using Application.UseCases.OrderCases.Commands.UpdateOrderCase;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles.OrderProfiles;

public class UpdateOrderMappingProfile : Profile
{
    public UpdateOrderMappingProfile()
    {
        CreateMap<UpdateOrderDto, UpdateOrderCommand>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.DeliveryDate, opt => opt.MapFrom(src => src.DeliveryDate))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

        CreateMap<UpdateOrderCommand, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.DeliveryDate, opt => opt.MapFrom(src => src.DeliveryDate))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
    }
}