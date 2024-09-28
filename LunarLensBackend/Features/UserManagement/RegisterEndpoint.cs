using FastEndpoints;
using LunarLensBackend.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace LunarLensBackend.Features.UserManagement;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public RegisterEndpoint(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task<RegisterResponse> ExecuteAsync(RegisterRequest req, CancellationToken ct)
    {
        var user = new IdentityUser { UserName = req.Email, Email = req.Email };
        var result = await _userManager.CreateAsync(user, req.Password);

        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync("BasicUser"))
            {
                await _roleManager.CreateAsync(new IdentityRole("BasicUser"));
            }

            await _userManager.AddToRoleAsync(user, "BasicUser");

            return new RegisterResponse
            {
                Id = Guid.Parse(user.Id),
                Email = user.Email
            };
        }
        
        // Collect error messages to log or return
        var errorMessages = result.Errors.Select(e => e.Description).ToList();
    
        var errorResponse = new RegisterResponse
        {
            Id = Guid.Empty, // Indicate failure
            Email = req.Email,
            Errors = errorMessages // Make sure to have an Errors property in RegisterResponse to hold these
        };
        
        await SendAsync(errorResponse, 400);
        return null;
    }
}