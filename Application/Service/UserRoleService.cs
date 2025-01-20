using Application.Interfaces;
using Application.Models.DTO;
using Application.Models;
using AutoMapper;
using Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IRoleManager<ApplicationUser> _roleManager;
        private readonly IMapper _mapper;


        public UserRoleService(IRoleManager<ApplicationUser> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }


        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
        {
            var roles = await _roleManager.GetAllRolesAsync();
            return _mapper.Map<IEnumerable<RoleDTO>>(roles);
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            return await _roleManager.CreateRoleAsync(roleName) > 0;
        }

        public async Task<List<string>> GetUserRolesByIdAsync(Guid userId)
        {
            return await _roleManager.GetUserRolesByIdAsync(userId);
        }


        public async Task<bool> AssingRangeRolesAsync(Guid userId, Dictionary<string, bool> roles)
        {
            var assignedResult = await _roleManager.AssingRangeRolesAsync(userId, roles);

            if (assignedResult)
            {
                return true;
            }
            return false;
        }
    }
}
