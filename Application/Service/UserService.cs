using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Interfaces;
using Application.Models;
using Application.Models.AuthModels;
using Application.Models.DTO;
using AutoMapper;
using Entities.Interfaces;
using Entities.Models;

namespace Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenManager _tokenManager;

        public UserService(IUserManager<ApplicationUser> userManager,IMapper mapper, IRefreshTokenManager tokenManager)
        {
            _userManager = userManager;
            //_userBlockScheduler = userBlockScheduler;
            _mapper = mapper;
            _tokenManager = tokenManager;
        }

        public async Task<(IEnumerable<UserDTO>, int)> GetAllUsersAsync(int page, int offset, string searchQuery)
        {

            Expression<Func<ApplicationUser, bool>> filter = null;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                filter = user =>
                    (user.FirstName.Contains(searchQuery) ||
                     user.LastName.Contains(searchQuery)) ||
                    user.Email.Contains(searchQuery);
            }

            var (users, totalPages) = await _userManager.GetAllUsersAsync(page, offset, filter);

            return (_mapper.Map<IEnumerable<UserDTO>>(users),totalPages);

        }

        public async Task<(IEnumerable<UserDTO>, int)> GetUsersByAllRolesAsync(int page, int offset, string[] roles, string searchQuery)
        {
            Expression<Func<ApplicationUser, bool>> filter = null;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                filter = user =>
                    (user.FirstName.Contains(searchQuery) ||
                     user.LastName.Contains(searchQuery)) ||
                    user.Email.Contains(searchQuery);
            }
            var (users,totalPages) = await _userManager.GetUsersByAllRolesAsync(page, offset, roles, filter);
            return users == null ? (Enumerable.Empty<UserDTO>(),0) : (_mapper.Map<IEnumerable<UserDTO>>(users), totalPages);
        }

        public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.GetUserByIdAsync(userId);

            return user == null ? null : _mapper.Map<UserDTO>(user);
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.GetUserByEmailAsync(email);

            return user;
        }

        public async Task<UserDTO> CreateUserAsync(RegistrationModel userDto)
        {
            if (await _userManager.IsUserExistAsync(userDto.Email, userDto.UserName))
            {
                throw new ArgumentException("Такой пользователь уже зарегистрирован.");
            }

            var applicationUser = _mapper.Map<ApplicationUser>(userDto);
            applicationUser.PasswordHash = PasswordHasher.HashPassword(applicationUser.PasswordHash);
            var createdUser = await _userManager.CreateUserAsync(applicationUser);
            return _mapper.Map<UserDTO>(createdUser);
        }

        public async Task<UserDTO> UpdateUserAsync(Guid userId, UserDTO userDto)
        {
            var existingUser = await _userManager.GetUserByIdAsync(userId);
            if (existingUser == null)
            {
                throw new ArgumentException($"Пользователь с ID {userId} не найден.");
            }

            existingUser.FirstName = userDto.FirstName;
            existingUser.LastName = userDto.LastName;
            existingUser.Email = userDto.Email;
            existingUser.UserName = userDto.UserName;

            await _userManager.UpdateUserAsync(userId, existingUser);

            return _mapper.Map<UserDTO>(existingUser);
        }


        public async Task CreateUserRangeAsync(params UserDTO[] userDtos)
        {
            var users = new List<ApplicationUser>();

            foreach (var item in userDtos)
            {
                var user = _mapper.Map<ApplicationUser>(item);

                if (!await _userManager.IsUserExistAsync(user.Email, user.UserName))
                {
                    users.Add(user);
                }
            }

            if (users.Any())
            {
                await _userManager.CreateUserRangeAsync(users.ToArray());
            }
        }

        public async Task BlockUserAsync(Guid userId, DateTime blockUntil = default)
        {

            var user = await _userManager.GetUserByIdAsync(userId);
            if(user == null)
            {
                throw new ArgumentException($"Пользователь с ID {userId} не найден.");
            }

            if(blockUntil == default)
            {
                blockUntil = DateTime.MaxValue;
            }

            user.BlockedUntil = blockUntil;
            user.IsLocked = true;
            await _userManager.UpdateUserAsync(userId, user);
            await _tokenManager.RevokeAllRefreshTokens(userId);

        }

        public async Task UnblockUserAsync(Guid userId)
        {
            var user = await _userManager.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"Пользователь с ID {userId} не найден.");
            }

            user.BlockedUntil = DateTime.MinValue;
            user.IsLocked= false;
            await _userManager.UpdateUserAsync(userId, user);
        }
    }
}
