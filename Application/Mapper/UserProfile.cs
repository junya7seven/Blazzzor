using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models;
using Application.Models.AuthModels;
using Application.Models.DTO;
using AutoMapper;
using Entities.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Mapper
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role))); 

            CreateMap<ApplicationUser, UserDTO>()
                .IncludeBase<User, UserDTO>(); 

            CreateMap<UserDTO, ApplicationUser>()
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore()) 
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore()); 

            CreateMap<Role, RoleDTO>();

            CreateMap<RoleDTO, Role>();
            
            CreateMap<RegistrationModel,ApplicationUser>();

        }
    }
}
