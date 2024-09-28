using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using LunarLensBackend.Database;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Extensions.Configuration;

namespace LunarLensBackend.Features.UserManagement;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly Context context;
    private readonly IConfiguration configuration;

    public LoginEndpoint(Context context, IConfiguration configuration)
    {
        this.context = context;
        this.configuration = configuration;  // Inject IConfiguration to access environment variables
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task<LoginResponse> ExecuteAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Email.ToUpper() == req.Email.ToUpper()
                                                                && x.Password == req.Password);
        if (user == null)
            ThrowError("Could not log in", StatusCodes.Status404NotFound);

        // Access the JWT secret from environment variables via IConfiguration
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

        var jwt = JwtBearer.CreateToken(options =>
        {
            options.SigningKey = jwtSecret;  // Use the JWT secret from the environment
            options.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            options.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Name, user.Email));
            //options.User.Roles.Add(user.Role);
            options.ExpireAt = DateTime.UtcNow.AddDays(7);
        });
        
        return new LoginResponse(jwt, user.Email);
    }
}

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Jwt, string Email);