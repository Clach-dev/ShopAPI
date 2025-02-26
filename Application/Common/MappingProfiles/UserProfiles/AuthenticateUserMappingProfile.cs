using Application.Common.Dtos.User;
using Application.UseCases.UserCases.Commands.AuthenticationUserCase;
using AutoMapper;

namespace Application.Common.MappingProfiles.UserProfiles;

public class AuthenticateUserMappingProfile : Profile
{
    public AuthenticateUserMappingProfile()
    {
        CreateMap<AuthenticationUserDto, AuthenticationUserCommand>()
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
    }
}