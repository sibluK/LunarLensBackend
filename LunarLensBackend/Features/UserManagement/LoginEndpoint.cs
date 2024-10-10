using FastEndpoints;
using LunarLensBackend.Database;
using LunarLensBackend.DTOs;
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
        
        return new LoginResponse(accessToken, refreshToken, DateTime.UtcNow.AddMinutes(15));
    }
}