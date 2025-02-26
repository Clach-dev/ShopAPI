using Application.Common.Dtos.Category;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles.CategoryProfiles;

public class ReadCategoryMappingProfile : Profile
{
    public ReadCategoryMappingProfile()
    {
        CreateMap<Category, ReadCategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
    }
}