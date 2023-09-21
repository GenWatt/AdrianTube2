
using System.Security.Claims;

namespace AdrianTube2.Services.Interfaces;

public interface IAuthService
{
    public void RedirectToLoginPage();

    public Task<ClaimsPrincipal> LoginAsync();

    public Task<bool> LogoutAsync();
}
