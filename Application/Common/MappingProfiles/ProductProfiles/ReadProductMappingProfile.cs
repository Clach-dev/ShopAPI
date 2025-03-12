using Application.Common.Dtos.Product;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles.ProductProfiles;

public class ReadProductMappingProfile : Profile
{
    public ReadProductMappingProfile()
    {
        CreateMap<Product, ReadProductDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.ImageUri, opt => opt.MapFrom(src => src.ImageUri));
    }
}