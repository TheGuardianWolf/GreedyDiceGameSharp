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
        private IHttpContextAccessor HttpContextAccessor { get; set; }

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
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, $"{Guid.NewGuid()}"),
                        new Claim(ClaimTypes.Role, $"{UserRoles.Anonymous}")
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContextAccessor.HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));
                }
                else
                {
                    NavigationManager.NavigateTo("/forbidden");
                }
            }
        }
    }
}
