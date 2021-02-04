using AutoMapper;
using JLL.PizzaProblem.API.Dtos;
using JLL.PizzaProblem.Domain;

namespace JLL.PizzaProblem.API.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<User, UserForCreationDto>().ReverseMap();
            CreateMap<User, UserForPatchDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
