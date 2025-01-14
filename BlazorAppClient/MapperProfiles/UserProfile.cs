using AutoMapper;
using Shared;

namespace BlazorAppClient.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDTO,UpdateUser>().ReverseMap();
            CreateMap<UserDTO,UserTable>().ReverseMap();
        }
    }
}
