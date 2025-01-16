using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Interfaces
{
    public interface IRefreshTokenManager
    {
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, Guid userId);
        Task<bool> AddRefreshTokenAsync(RefreshToken refreshToken);
    }
}
