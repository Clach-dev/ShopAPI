using Application.Common.Dtos.Category;
using Application.UseCases.CategoryCases.Commands.CreateCategoryCase;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles.CategoryProfiles;

public class CreateCategoryMappingProfile : Profile
{
    public CreateCategoryMappingProfile()
    {
         CreateMap<CreateCategoryDto, CreateCategoryCommand>()
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
         
         CreateMap<CreateCategoryCommand, Category>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
    }
}