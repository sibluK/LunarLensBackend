using FastEndpoints;
using LunarLensBackend.Database;
using LunarLensBackend.Utility;
using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Features.UserManagement;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenGeneration _tokenGeneration;

    public LoginEndpoint(UserManager<ApplicationUser> userManager, TokenGeneration tokenGeneration)
    {
        _userManager = userManager;
        _tokenGeneration = tokenGeneration;
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task<LoginResponse> ExecuteAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, req.Password))
        {
            ThrowError("Invalid login credentials", StatusCodes.Status401Unauthorized);
        }
        
        var accessToken = await _tokenGeneration.GenerateAccessTokenAsync(user);
        var refreshToken = await _tokenGeneration.GenerateRefreshTokenAsync(user); 
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();
        
        return new LoginResponse(accessToken, refreshToken, DateTime.UtcNow.AddMinutes(15), role);
    }
}

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class LoginResponse
{
    public string AccessToken { get; set; }  
    public string RefreshToken { get; set; }
    public DateTime Expires { get; set; }
    public string Role { get; set; }

    public LoginResponse(string access, string refresh, DateTime expires, string role)
    {
        AccessToken = access;
        RefreshToken = refresh;
        Expires = expires;
        Role = role;
    }
}