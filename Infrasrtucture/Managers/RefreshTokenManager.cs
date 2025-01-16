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
    public class RefreshTokenManager<TUser> : IRefreshTokenManager where TUser : User
    {
        private readonly ApplicationDbContext<TUser> _context;
        public RefreshTokenManager(ApplicationDbContext<TUser> context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, Guid userId)
        {
            var existRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken && t.UserId == userId);
            return existRefreshToken;
        }
        public async Task<bool> AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            return await _context.SaveChangesAsync() > 0;

        }
    }
}
