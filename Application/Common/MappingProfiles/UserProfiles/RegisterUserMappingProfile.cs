using Application.Common.Dtos.User;
using Application.UseCases.UserCases.Commands.RegisterUserCase;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.MappingProfiles.UserProfiles;

public class RegisterUserMappingProfile : Profile
{
    public RegisterUserMappingProfile()
    {
        CreateMap<RegisterUserDto, RegisterUserCommand>()
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        
        CreateMap<RegisterUserCommand, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Roles.User));
    }
}