using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Entities.Interfaces
{
    public interface IUserManager<TUser> where TUser : User
    {
        Task<IEnumerable<TUser?>> GetAllUsersAsync();
        Task<TUser?> GetUserByIdAsync(Guid id);
        Task<TUser?> GetUserByEmailAsync(string email);
        Task<TUser> CreateUserAsync(TUser user);
        Task<int> CreateUserRangeAsync(params TUser[] user);
        Task<int> UpdateUserAsync(Guid userId, TUser user);
        Task<int> BlockUserByEmailAsync(string email, TimeSpan duration);
        Task<int> BlockUserByIdAsync(Guid id, TimeSpan duration);
        Task<bool> CheckUserSessions(Guid userId);
    }
}
