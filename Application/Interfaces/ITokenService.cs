﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Models.AuthModels;
using Application.Models;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        Task<RequestAccess> GenerateTokens(ApplicationUser user);
        Task<ClaimsPrincipal> GetClaimsPrincipalAsync(string token);
    }

}
