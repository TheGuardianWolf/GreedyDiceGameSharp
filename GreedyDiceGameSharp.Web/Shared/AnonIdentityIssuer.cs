using GreedyDiceGameSharp.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GreedyDiceGameSharp.Web.Shared
{
    public class AnonIdentityIssuer: ComponentBase
    {
        [Inject]
        private IAuthService AuthService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (NavigationManager is not null && AuthenticationStateTask is not null)
            {
                AuthenticationState authenticationState = await AuthenticationStateTask;

                if (authenticationState?.User?.Identity is null || !authenticationState.User.Identity.IsAuthenticated)
                {
                    await AuthService.IssueAnonIdentity();
                }
                else
                {
                    NavigationManager.NavigateTo("/forbidden");
                }
            }
        }
    }
}
