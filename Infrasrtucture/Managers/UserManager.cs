using Entities.Interfaces;
using Entities.Models;
using Infrasrtucture.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrasrtucture.Managers
{
    public class UserManager<TUser> : IUserManager<TUser> where TUser : User
    {
        private readonly ApplicationDbContext<TUser> _context;

        public UserManager(ApplicationDbContext<TUser> context)
        {
            _context = context;
        }


        public async Task<IEnumerable<TUser>> GetAllUsersAsync()
        {
            var users = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role).ToListAsync();
            return users ?? Enumerable.Empty<TUser>();
        }
        public async Task<TUser?> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);
            return user;
        }
        public async Task<TUser?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.NormalEmail == email.ToLower());
            return user;
        }
        public async Task<TUser> CreateUserAsync(TUser user)
        {
            var existUser = await _context.Users.AnyAsync(u =>
            u.NormalUserName == user.Email.ToLower()
            || u.NormalUserName == user.UserName.ToLower());

            if (existUser)
            {
                throw new ArgumentException($"Такой пользователь уже зарегистрирован");
            }

            user.NormalEmail = user.Email.ToLower();
            user.NormalUserName = user.UserName.ToLower();
            user.PasswordHash = user.PasswordHash;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<int> CreateUserRangeAsync(params TUser[] users)
        {
            if (!users.Any())
            {
                throw new ArgumentNullException("User не может быть пустым");
            }
            foreach (var item in users)
            {
                item.NormalUserName = item.UserName.ToLower();
                item.NormalEmail = item.Email.ToLower();
            }
            await _context.Users.AddRangeAsync(users);
            return await _context.SaveChangesAsync();
        }


        public async Task<int> UpdateUserAsync(Guid userId, TUser user)
        {
            var existUser = await GetUserByIdAsync(userId);
            if (existUser == null)
            {
                throw new KeyNotFoundException($"Такой пользователь не найден");
            }

            var checkData = await _context.Users.AnyAsync(u => (u.NormalUserName == user.UserName.ToLower()
            || u.NormalEmail == user.Email.ToLower())
            && u.UserId != existUser.UserId);

            if (checkData)
            {
                throw new ArgumentException($"Такие почта или имя уже зарегистрированы");
            }

            if (user.UserName != null)
            {
                existUser.UserName = user.UserName;
                existUser.NormalUserName = user.UserName.ToLower();
            }
            if (user.Email != null)
            {
                existUser.Email = user.Email;
                existUser.NormalEmail = user.Email.ToLower();
            }
            existUser.LastUpdateAt = DateTime.Now;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> BlockUserByEmailAsync(string email, TimeSpan duration = default)
        {
            if (duration == default)
            {
                duration = TimeSpan.FromDays(365 * 100);
            }
            var existUser = await GetUserByEmailAsync(email);
            if (existUser == null)
            {
                throw new KeyNotFoundException($"Такой пользователь не найден");
            }

            existUser.IsLocked = true;
            existUser.BlockedUntil = DateTime.Now.Add(duration);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> BlockUserByIdAsync(Guid userId, TimeSpan duration = default)
        {
            if (duration == default)
            {
                duration = TimeSpan.FromDays(365 * 100);
            }
            var existUser = await GetUserByIdAsync(userId);
            if (existUser == null)
            {
                throw new KeyNotFoundException($"Такой пользователь не найден");
            }

            existUser.IsLocked = true;
            existUser.BlockedUntil = DateTime.Now.Add(duration);
            return await _context.SaveChangesAsync();
        }
    }
}
