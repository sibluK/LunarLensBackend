using FastEndpoints;
using LunarLensBackend.Database;
using LunarLensBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Features.Admin;

public class PromoteUserEndpoint : Endpoint<PromoteUserRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public PromoteUserEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/admin/promote");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(PromoteUserRequest req, CancellationToken ct)
    {
        Console.WriteLine($"Promote request for {req.UserEmail} to role {req.NewRole}");
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