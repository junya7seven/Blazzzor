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
    public class RoleManager<TUser> : IRoleManager<TUser> where TUser : User
    {
        private readonly UserManager<TUser> _UserManager;
        private readonly ApplicationDbContext<TUser> _context;
        public RoleManager(UserManager<TUser> userManager, ApplicationDbContext<TUser> context)
        {
            _UserManager = userManager;
            _context = context;
        }

        public async Task<IEnumerable<Role?>> GetAllRolesAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            return roles ?? Enumerable.Empty<Role>();
        }
        public async Task<int> CreateRoleAsync(string roleName)
        {
            var existRole = await _context.Roles.AnyAsync(r => r.NormalName == roleName.ToLower());
            if (existRole)
            {
                return 0;
            }
            var role = new Role
            {
                NormalName = roleName.ToLower(),
                Name = roleName
            };
            await _context.Roles.AddAsync(role);
            return await _context.SaveChangesAsync();
        }
        public async Task<List<string>> GetUserRolesByIdAsync(Guid userId)
        {
            var existUser = await _context.Users.FindAsync(userId);
            if(existUser == null)
            {
                throw new KeyNotFoundException($"Такой пользователь не существует");
            }
            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.NormalName) 
                .ToListAsync();
            return roles;
        }
        public async Task<List<string>> GetUserRolesByEmailAsync(string Email)
        {
            var existUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (existUser == null)
            {
                throw new KeyNotFoundException($"Такой пользователь не существует");
            }
            var roles = await _context.UserRoles
               .Where(ur => ur.UserId == existUser.UserId)
               .Select(ur => ur.Role.NormalName)
               .ToListAsync();
            return roles;
        }
        public async Task<IEnumerable<TUser>> GetUsersByRoleAsync(string role)
        {
            var existRole = await _context.Roles.AnyAsync(r => r.NormalName == role.ToLower());
            if (!existRole)
            {
                throw new KeyNotFoundException($"Такой роли не существует");
            }

            var roleId = await _context.Roles
                .Where(r => r.NormalName == role.ToLower())
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var users = await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.User) 
                .ToListAsync();

            return users.Cast<TUser>(); 
        }


        /// <summary>
        /// Назначение роли пользователю. Если роль не существует, она создастся. Если не будет найден пользователь по его userId выбросится исключение
        /// ArgumentException. При успешном добавлении роли пользователю вернется - 1, в ином случае - 0
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <param name="roleName">название роли</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> AssignRoleByIdAsync(Guid userId, string roleName)
        {
            // Находим пользователя по идентификатору
            var existUser = await _context.Users.FindAsync(userId);
            if (existUser == null)
            {
                throw new KeyNotFoundException($"Такой пользователь не существует");
            }

            var existRole = await _context.Roles.FirstOrDefaultAsync(r => r.NormalName == roleName.ToLower());

            if (existRole == null)
            {
                await CreateRoleAsync(roleName); 
                existRole = await _context.Roles.FirstOrDefaultAsync(r => r.NormalName == roleName.ToLower()); 
            }

            var userRoleExists = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == existRole.Id);

            if (!userRoleExists)
            {
                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = existRole.Id
                };

                await _context.UserRoles.AddAsync(userRole);

                return await _context.SaveChangesAsync(); 
            }

            return 0; 
        }

        public async Task<int> AssignRoleByEmailAsync(string email, string roleName)
        {
            var existUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existUser == null)
            {
                throw new KeyNotFoundException($"Такой пользователь не существует");
            }

            var existRole = await _context.Roles.FirstOrDefaultAsync(r => r.NormalName == roleName.ToLower());

            if (existRole == null)
            {
                await CreateRoleAsync(roleName);
                existRole = await _context.Roles.FirstOrDefaultAsync(r => r.NormalName == roleName.ToLower());
            }

            var userRoleExists = await _context.UserRoles.AnyAsync(ur => ur.UserId == existUser.UserId && ur.RoleId == existRole.Id);

            if (!userRoleExists)
            {
                var userRole = new UserRole
                {
                    UserId = existUser.UserId,
                    RoleId = existRole.Id
                };

                await _context.UserRoles.AddAsync(userRole);

                return await _context.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<bool> AssingRangeRolesAsync(Guid userId, Dictionary<string,bool> roles)
        {
            try
            {
                foreach (var role in roles)
                {
                    if (role.Value)
                    {
                        await AssignRoleByIdAsync(userId, role.Key);
                    }
                    else
                    {
                        await RevokeRoleAsync(userId, role.Key);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<int> RevokeRoleAsync(Guid userId, string roleName)
        {
            var existRole = await _context.Roles.FirstOrDefaultAsync(r => r.NormalName == roleName.ToLower());

            if (existRole == null)
            {
                return 0;
            }

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == existRole.Id);

            if (userRole == null)
            {
                return 0;
            }

            _context.UserRoles.Remove(userRole);

            return await _context.SaveChangesAsync(); 
        }
    }
}
