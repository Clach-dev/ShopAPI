using Application.Common.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.MappingProfiles;

public class PageInfoMappingProfile : Profile
{
    public PageInfoMappingProfile()
    {
        CreateMap<PageInfoDto, PageInfo>()
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize));
    }
}