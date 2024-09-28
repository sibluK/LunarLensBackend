using FastEndpoints;
using LunarLensBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Features.Admin;

public class PromoteUserEndpoint : Endpoint<PromoteUserRequest>
{
    private readonly UserManager<IdentityUser> _userManager;

    public PromoteUserEndpoint(UserManager<IdentityUser> userManager)
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
            await _userManager.RemoveFromRolesAsync(user, currentRoles); // Remove current roles

            var result = await _userManager.AddToRoleAsync(user, req.NewRole);
            if (result.Succeeded)
            {
                await SendOkAsync($"User promoted to {req.NewRole}");
            }
            else
            {
                // Return a 400 Bad Request response
                await SendAsync(new { Error = "Failed to promote user" }, 400);
            }
        }
        else
        {
            // Return a 400 Bad Request response
            await SendAsync(new { Error = "User not found" }, 400);
        }
    }
}