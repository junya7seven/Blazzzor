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


        public async Task<(IEnumerable<TUser>,int)> GetAllUsersAsync(int page, int offset)
        {
            var lastPage = await _context.Users
            .CountAsync() / offset;

            var users = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Skip((page - 1) * offset)
            .Take(offset)
            .ToListAsync();
            return (users ?? Enumerable.Empty<TUser>(),lastPage);
        }

        public async Task<(IEnumerable<TUser>, int)> GetUsersByAllRolesAsync(int page, int offset, string[] roles)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => roles.All(r => u.UserRoles.Any(ur => ur.Role.NormalName == r.ToLower())));

            var totalUsers = await query.CountAsync();

            var users = await query
                .Skip((page - 1) * offset)
                .Take(offset)
                .ToListAsync();

            return (users, totalUsers);
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
            var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.NormalEmail == email.ToLower());
            return user;
        }

        public async Task<bool> IsUserExistAsync(string email, string userName)
        {
            return await _context.Users.AnyAsync(u =>
                u.NormalEmail == email.ToLower() ||
                u.NormalUserName == userName.ToLower());
        }

        public async Task<TUser> CreateUserAsync(TUser user)
        {
            var existUser = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return existUser.Entity;
        }
        public async Task CreateUserRangeAsync(params TUser[] users)
        {
            foreach(var user in users)
            {
                user.NormalEmail = user.Email.ToLower();
                user.NormalUserName = user.UserName.ToLower();
            }

            await _context.Set<TUser>().AddRangeAsync(users);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateUserAsync(Guid userId, TUser user)
        {
            var existingUser = await _context.Set<TUser>().FindAsync(userId);
            if (existingUser == null)
            {
                throw new ArgumentException($"Пользователь с ID {userId} не найден.");
            }

            existingUser.LastUpdateAt = DateTime.Now;

            var entry = _context.Entry(existingUser);

            entry.CurrentValues.SetValues(user);

            entry.Property(e => e.UserId).IsModified = false;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TUser>> GetAllLockedUsersAsync()
        {
            var lockUsers = await _context.Users.Where(u => u.IsLocked == true).ToListAsync();
            return lockUsers;
        }
    }
}
