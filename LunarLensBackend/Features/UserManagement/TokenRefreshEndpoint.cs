using FastEndpoints;
using LunarLensBackend.Database;
using LunarLensBackend.Utility;
using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Features.UserManagement;

public class TokenRefreshEndpoint : Endpoint<RefreshRequest, RefreshResponse>
{
    public readonly TokenGeneration _tokenGeneration;

    public TokenRefreshEndpoint(TokenGeneration tokenGeneration, UserManager<ApplicationUser> userManager)
    {
        _tokenGeneration = tokenGeneration;
    }
    
    public override void Configure()
    {
        Post("/auth/refresh");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RefreshRequest req, CancellationToken ct)
    {
        try
        {
            var newAccessToken = await _tokenGeneration.HandleRefreshTokenAsync(req.refreshToken, ct);
            
            await SendAsync(new RefreshResponse(newAccessToken, DateTime.UtcNow));
        }
        catch (UnauthorizedAccessException ex)
        {
            ThrowError(ex.Message, StatusCodes.Status401Unauthorized);
        }
    }
}

public class RefreshRequest
{
    public string refreshToken { get; set; }
}

public class RefreshResponse
{
    public string AccessToken { get; set; }
    public DateTime ExpiresIn { get; set; }

    public RefreshResponse(string accessToken, DateTime expiresIn)
    {
        AccessToken = accessToken;
        ExpiresIn = expiresIn;
    }
}