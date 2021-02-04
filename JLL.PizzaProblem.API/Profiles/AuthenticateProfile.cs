using AutoMapper;
using JLL.PizzaProblem.API.Dtos;
using JLL.PizzaProblem.Domain;

namespace JLL.PizzaProblem.API.Profiles
{
    public class AuthenticateProfile : Profile
    {
        public AuthenticateProfile()
        {
            CreateMap<AuthenticateRequest, AuthenticateRequestDto>().ReverseMap();
            CreateMap<AuthenticateResponse, AuthenticateResponseDto>().ReverseMap();
            CreateMap<AuthenticateResponse, UserDto>().ReverseMap();
        }
    }
}
