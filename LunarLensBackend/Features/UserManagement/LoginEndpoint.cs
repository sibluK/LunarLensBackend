using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using LunarLensBackend.DTOs;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace LunarLensBackend.Features.UserManagement;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly UserManager<IdentityUser> _userManager;

    public LoginEndpoint(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
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

        // Retrieve user roles (optional but useful)
        var roles = await _userManager.GetRolesAsync(user);
        Console.WriteLine($"Roles for user {user.Email}: {string.Join(", ", roles)}");

        // Generate JWT token
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

        var jwt = JwtBearer.CreateToken(options =>
        {
            options.SigningKey = jwtSecret;
            options.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            options.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Name, user.Email));

            // Add roles as claims
            foreach (var role in roles)
            {
                
                options.User.Claims.Add(new Claim(ClaimTypes.Role, role));
            }

            options.ExpireAt = DateTime.UtcNow.AddDays(7);
        });

        // Return the login response with JWT and user roles
        return new LoginResponse(jwt, user.Email, roles.ToArray());
    }
}