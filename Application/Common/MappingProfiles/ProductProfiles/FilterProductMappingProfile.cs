using Application.Common.Dtos.Product;
using Application.UseCases.ProductCases.Queries.GetProductsByFilterQuery;
using AutoMapper;

namespace Application.Common.MappingProfiles.ProductProfiles;

public class FilterProductMappingProfile : Profile
{
    public FilterProductMappingProfile()
    {
        CreateMap<GetProductsByFilterDto, GetProductsByFilterQuery>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.MinPrice, opt => opt.MapFrom(src => src.MinPrice))
            .ForMember(dest => dest.MaxPrice, opt => opt.MapFrom(src => src.MaxPrice))
            .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.CategoryIds))
            .ForMember(dest => dest.PageInfoDto, opt => opt.MapFrom(src => src.PageInfoDto));
    }
}