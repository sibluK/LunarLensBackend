using FastEndpoints;
using LunarLensBackend.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LunarLensBackend.Features.User;

public class GetUsersByRoleEndpoint : Endpoint<GetUsersByRoleRequest, GetUsersByRoleResponse>
{
    private readonly Context _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUsersByRoleEndpoint(Context context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager; 
    }

    public override void Configure()
    {
        Get("/users/{Role}");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(GetUsersByRoleRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Role))
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var users = await _userManager.GetUsersInRoleAsync(req.Role);

        var response = new GetUsersByRoleResponse
        {
            Users = users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = req.Role
            }).ToList()
        };

        await SendAsync(response, StatusCodes.Status200OK, ct);
    }
}

public class GetUsersByRoleRequest
{
    [FromRoute]
    public string Role { get; set; }
}

public class GetUsersByRoleResponse
{
    public List<UserDto> Users { get; set; }
}

public class UserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}