using Application.Models;
using AutoMapper;
using BlazorTemplate.Models;

namespace BlazorTemplateAPI.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegistrationModel, ApplicationUser>().ReverseMap();
            CreateMap<UpdateUser,ApplicationUser>().ReverseMap();
        }
    }
}
