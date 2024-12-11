using Moq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using LunarLensBackend.Database;
using LunarLensBackend.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace LunarLensBackend.Tests;

public class TokenGenerationTests
{
    [Fact]
    public void GenerateRefreshToken_ShouldReturnValidBase64String()
    {
        var tokenGeneration = new TokenGeneration(null, null, null);

        var refreshToken = tokenGeneration.GenerateRefreshToken();

        Assert.NotNull(refreshToken);
        Assert.False(string.IsNullOrWhiteSpace(refreshToken));
        Assert.True(IsBase64String(refreshToken));
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnUniqueTokens()
    {
        var tokenGeneration = new TokenGeneration(null, null, null);

        var token1 = tokenGeneration.GenerateRefreshToken();
        var token2 = tokenGeneration.GenerateRefreshToken();

        Assert.NotEqual(token1, token2);
    }

    private bool IsBase64String(string value)
    {
        var buffer = new Span<byte>(new byte[value.Length]);
        return Convert.TryFromBase64String(value, buffer, out _);
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_ReturnsValidToken_withRoles()
    {
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        var configurationMock = new Mock<IConfiguration>();

        var user = new ApplicationUser
        {
            Id = "123",
            Email = "test@example.com"
        };
        
        userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "Admin", "BasicUser", "Editor", "Writer" });

        configurationMock.Setup(c => c.GetSection("Jwt:Issuer").Value)
            .Returns("TestIssuer");
        configurationMock.Setup(c => c.GetSection("Jwt:Audience").Value)
            .Returns("TestAudience");
        
        Environment.SetEnvironmentVariable("JWT_SECRET",
            "your-secret-keyyour-secret-keyyour-secret-keyyour-secret-keyyour-secret-key");

        var tokenService = new TokenGeneration(userManagerMock.Object, configurationMock.Object, null);
        
        var result = await tokenService.GenerateAccessTokenAsync(user);
        
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(result) as JwtSecurityToken;

        var claims = jsonToken?.Claims;

        Assert.NotNull(result);
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Admin");
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "BasicUser");
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Editor");
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Writer");
        Assert.Contains(claims,
            claim => claim.Type == JwtRegisteredClaimNames.Email && claim.Value == "test@example.com");
        Assert.Contains(claims, claim => claim.Type == JwtRegisteredClaimNames.Sub && claim.Value == "123");

        Environment.SetEnvironmentVariable("JWT_SECRET", null);
    }
}