using Application.Common.Dtos.Product;
using Application.UseCases.ProductCases.Commands.CreateProductCase;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles.ProductProfiles;

public class CreateProductMappingProfile : Profile
{
    public CreateProductMappingProfile()
    {
        CreateMap<CreateProductDto, CreateProductCommand>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.CategoryIds))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));
        
        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
    }
}