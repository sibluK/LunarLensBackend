using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace LunarLensBackend.Features.Admin;


public class TestAdminEndpoint : Endpoint<EmptyRequest>
{
    public override void Configure()
    {
        Get("/admin/test");
        Policies("AdminOnly");
        ResponseCache(60);
        Throttle(
            hitLimit: 120,
            durationSeconds: 60
        );
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        Console.WriteLine("Admin endpoint accessed successfully!"); 
        await SendOkAsync("Access granted at " + DateTime.UtcNow);
    }
}