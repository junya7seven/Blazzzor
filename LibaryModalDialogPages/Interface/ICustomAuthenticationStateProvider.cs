using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibaryModalDialogPages.Interface
{
    public interface ICustomAuthenticationStateProvider
    {
        Task<AuthenticationState> GetAuthenticationStateAsync();
    }
}
