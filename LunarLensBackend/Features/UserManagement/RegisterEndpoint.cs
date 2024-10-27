using FastEndpoints;
using LunarLensBackend.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace LunarLensBackend.Features.UserManagement;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public RegisterEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task<RegisterResponse> ExecuteAsync(RegisterRequest req, CancellationToken ct)
    {
        var user = new ApplicationUser() { UserName = req.Email, Email = req.Email };
        var result = await _userManager.CreateAsync(user, req.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "BasicUser");

            return new RegisterResponse
            {
                Id = Guid.Parse(user.Id),
                Email = user.Email
            };
        }
        
        var errorMessages = result.Errors.Select(e => e.Description).ToList();
    
        var errorResponse = new RegisterResponse
        {
            Id = Guid.Empty, 
            Email = req.Email,
            Errors = errorMessages
        };
        
        await SendAsync(errorResponse, 400);
        return null;
    }
}

public class RegisterResponse
{
    public Guid Id { get; set; }  
    public string Email { get; set; } 
    public List<string> Errors { get; set; }
}