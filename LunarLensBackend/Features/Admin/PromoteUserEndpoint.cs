using FastEndpoints;
using LunarLensBackend.Database;
using LunarLensBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Features.Admin;

public class PromoteUserEndpoint : Endpoint<PromoteUserRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public PromoteUserEndpoint(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public override void Configure()
    {
        Post("/admin/promote");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(PromoteUserRequest req, CancellationToken ct)
    {
        if (!await _roleManager.RoleExistsAsync(req.NewRole))
        {
            await SendAsync(new { Message = "Role does not exist" }, StatusCodes.Status400BadRequest);
            return;
        }
        
        var user = await _userManager.FindByEmailAsync(req.UserEmail);
        
        if (user != null)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, req.NewRole);
            if (result.Succeeded)
            {
                await SendOkAsync($"User promoted to {req.NewRole}");
            }
            else
            {
                await SendAsync(new { Error = "Failed to promote user" }, 400);
            }
        }
        else
        {
            await SendAsync(new { Error = "User not found" }, 400);
        }
    }
}