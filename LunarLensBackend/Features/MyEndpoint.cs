using FastEndpoints;
using LunarLensBackend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace LunarLensBackend.Features;

public class MyEndpoint : Endpoint<MyRequest, MyResponse>
{
    public override void Configure()
    {
        Post("/api/user/create");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(MyRequest req, CancellationToken ct)
    {
        Console.WriteLine($"Received request: {req.FirstName} {req.LastName}, Age: {req.Age}");
        await SendAsync(new()
        {
            FullName = req.FirstName + " " + req.LastName,
            IsOver18 = req.Age > 18
        });
    }
}
