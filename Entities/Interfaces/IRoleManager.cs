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

        Task<List<string>> GetUserRolesByIdAsync(Guid userId);

        Task<int> CreateRoleAsync(string roleName);

        Task<int> AssignRoleByIdAsync(Guid userId, string roleName);
        Task<int> RevokeRoleAsync(Guid roleId, string roleName);
        Task<bool> AssingRangeRolesAsync(Guid userId, Dictionary<string, bool> roles);
    }
}
