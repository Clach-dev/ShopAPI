using Application.Common.Dtos.Category;
using Application.UseCases.CategoryCases.Commands.UpdateCategoryCase;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles.CategoryProfiles;

public class UpdateCategoryMappingProfile : Profile
{
    public UpdateCategoryMappingProfile()
    {
        CreateMap<UpdateCategoryDto, UpdateCategoryCommand>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        
        CreateMap<UpdateCategoryCommand, Category>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
    }
}