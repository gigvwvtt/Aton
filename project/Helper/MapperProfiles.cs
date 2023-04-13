using AutoMapper;
using project.Dto;
using project.Models;

namespace project.Helper;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<User, UserForAdminDto>();
        CreateMap<CreateUserDto, User>();
    }
}