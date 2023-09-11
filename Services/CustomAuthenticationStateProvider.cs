using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace AdrianTube2.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AuthService _authService;
        
        public CustomAuthenticationStateProvider(AuthService authService)
        {
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var principal = await _authService.LoginAsync();
                
                return await Task.FromResult(new AuthenticationState(principal));
            }
            catch (Exception)
            {
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            }

            // return some sample data for testing
            // var identity = new ClaimsIdentity(new[]
            // {
            //     new Claim(ClaimTypes.Name, "Adrian"),
            //     new Claim(ClaimTypes.Role, "Administrator")
            // }, "Test authentication type");

            // var user = new ClaimsPrincipal(identity);

            // return await Task.FromResult(new AuthenticationState(user));
        }

        public async Task Logout()
        {
            var isLogOut = await _authService.LogoutAsync();

            if (isLogOut)
            {
                var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
                NotifyAuthenticationStateChanged(authState);
                _authService.RedirectToLoginPage();
            }
        }
    }
}