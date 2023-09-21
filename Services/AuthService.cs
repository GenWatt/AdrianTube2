using System.Net;
using System.Security.Claims;
using AdrianTube2.Models.Api;
using AdrianTube2.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace AdrianTube2.Services;

public class AuthService : IAuthService {

    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public AuthService(NavigationManager navigationManager, IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _navigationManager = navigationManager;
        _httpContextAccessor = httpContextAccessor;
        _http = httpClientFactory.CreateClient();
        _configuration = configuration;
    }

    public void RedirectToLoginPage()
    {
        // Redirect the user to the Node Auth server for authentication
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri).ToString();
        _navigationManager.NavigateTo($"{_configuration["AuthClientUrl"]}/login?callbackUrl={uri}&brand={_configuration["Appname"]}");
    }

    public async Task<ClaimsPrincipal> LoginAsync() {
        // Check if the user is authenticated
        string? access_token = "";
        _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue("access_token", out access_token);

        if (access_token == "")
        {
            RedirectToLoginPage();
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["AuthApiUrl"]}/profile");
        request.Headers.Add("Cookie", "access_token=" + access_token);

        var response = await _http.SendAsync(request);
        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();
        var isAuthenticated = response.StatusCode == HttpStatusCode.OK && userResponse is not null && userResponse.Success;

        if (!isAuthenticated)
        {
            RedirectToLoginPage();
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, userResponse.User.Username),
            new Claim(ClaimTypes.Email, userResponse.User.Email),
            new Claim(ClaimTypes.NameIdentifier, userResponse.User.Id.ToString()),
            new Claim(ClaimTypes.Role, userResponse.User.Role),
            new Claim("ProfilePicture", userResponse.User.ProfilePicture),
            new Claim("CoverPicture", userResponse.User.CoverPicture),
            new Claim("IsVerified", userResponse.User.IsVerified.ToString()),
            new Claim("CreatedAt", userResponse.User.CreatedAt.ToString()),
            new Claim("GoogleId", userResponse.User.GoogleId),
            new Claim("Active", userResponse.User.Active.ToString()),
            new Claim("IsLogged", userResponse.User.IsLogged.ToString()),
            new Claim("RefreshToken", userResponse.User.RefreshToken),
            new Claim("Provider", userResponse.User.Provider),
            new Claim("V", userResponse.User.V.ToString()),
            new Claim("Theme", userResponse.User.UserSettings.Theme),
            new Claim("Language", userResponse.User.UserSettings.Language),
        }, "custom");

        return new ClaimsPrincipal(identity); 
    }

    public async Task<bool> LogoutAsync()
    {
        string? access_token = "";
        _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue("access_token", out access_token);
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["AuthApiUrl"]}/logout");
        request.Headers.Add("Cookie", "access_token=" + access_token);

        var response = await _http.SendAsync(request);

        return response.StatusCode == HttpStatusCode.OK;
    }
}