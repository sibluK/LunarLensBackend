using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LunarLensBackend.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using FastEndpoints;
using LunarLensBackend.Entities;
using Microsoft.EntityFrameworkCore;

namespace LunarLensBackend.Utility;

public class TokenGeneration
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly Context _context;
    
    public TokenGeneration(UserManager<ApplicationUser> userManager, IConfiguration configuration, Context context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };
        
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _configuration.GetSection("Jwt:Issuer").Value,
            audience: _configuration.GetSection("Jwt:Audience").Value,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );
        
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return accessToken;
    }

    public async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
    {
        var existingTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in existingTokens)
        {
            token.IsRevoked = true; 
            token.RevokedDate = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
        
        var refreshToken = new RefreshToken
        {
            Token = GenerateRefreshToken(),
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            CreatedDate = DateTime.UtcNow,
            IsRevoked = false
        };
        
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        
        return refreshToken.Token;
    }

    public async Task<string> HandleRefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        var refreshTokenEntity = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked, ct);
        
        if (refreshTokenEntity == null || refreshTokenEntity.ExpiryDate < DateTime.UtcNow)
        {
            if (refreshTokenEntity != null)
            {
                refreshTokenEntity.IsRevoked = true;
                _context.RefreshTokens.Update(refreshTokenEntity);
                await _context.SaveChangesAsync(ct);
            }
            throw new UnauthorizedAccessException("Refresh token is expired or invalid.");
        }
        
        var user = await _userManager.FindByIdAsync(refreshTokenEntity.UserId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }
        
        var accessToken = await GenerateAccessTokenAsync(user);
        
        return accessToken;
    }
    
}