using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Entities.Interfaces
{
    public interface IUserManager<TUser> where TUser : User
    {
        Task<(IEnumerable<TUser>, int)> GetAllUsersAsync(int page, int offset, Expression<Func<TUser, bool>> filter = null);
        Task<(IEnumerable<TUser>, int)> GetUsersByAllRolesAsync(int page, int offset, string[] roles, Expression<Func<TUser, bool>> filter = null);
        Task<TUser?> GetUserByIdAsync(Guid id);
        Task<TUser?> GetUserByEmailAsync(string email);
        Task<IEnumerable<TUser>> GetAllLockedUsersAsync();

        Task<bool> IsUserExistAsync(string email, string userName);
        Task<TUser> CreateUserAsync(TUser user);
        Task CreateUserRangeAsync(params TUser[] user);
        Task UpdateUserAsync(Guid userId, TUser user);
    }
}
