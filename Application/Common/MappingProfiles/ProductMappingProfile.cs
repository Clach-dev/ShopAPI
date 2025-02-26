using Application.Common.Dtos.Product;
using Application.UseCases.ProductCases.Commands.CreateProductCase;
using Application.UseCases.ProductCases.Commands.UpdateProductCase;
using Application.UseCases.ProductCases.Queries.GetProductsByFilterQuery;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<CreateProductDto, CreateProductCommand>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.CategoryIds));
        
        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
     
        CreateMap<Product, ReadProductDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
        
        CreateMap<UpdateProductDto, UpdateProductCommand>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.CategoryIds));
        
        CreateMap<UpdateProductCommand, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Categories, opt => opt.Ignore());

        CreateMap<GetProductsByFilterDto, GetProductsByFilterQuery>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.MinPrice, opt => opt.MapFrom(src => src.MinPrice))
            .ForMember(dest => dest.MaxPrice, opt => opt.MapFrom(src => src.MaxPrice))
            .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.CategoryIds))
            .ForMember(dest => dest.PageInfoDto, opt => opt.MapFrom(src => src.PageInfoDto));
    }
}