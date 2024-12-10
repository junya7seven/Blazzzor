using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Entities.Interfaces
{
    public interface IRoleManager<TUser> where TUser : User
    {
        Task<IEnumerable<Role?>> GetAllRolesAsync();
        Task<int> CreateRoleAsync(string roleName);
        Task<List<string>> GetUserRolesByIdAsync(Guid userId);
        Task<List<string>> GetUserRolesByEmailAsync(string email);
        Task<IEnumerable<TUser>> GetUsersByRoleAsync(string role);
        Task<int> AssignRoleByIdAsync(Guid userId, string roleName);
        Task<int> AssignRoleByEmailAsync(string email, string roleName);
        Task<int> RevokeRoleAsync(Guid roleId, string roleName);
    }
}
