/*
using FastEndpoints;
using LunarLensBackend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace LunarLensBackend.Features.Editor;

[Authorize(Roles = "Editor")]
public class CreateArticleEndpoint : Endpoint<CreateArticleRequest>
{
    
    public override void Configure()
    {
        Post("/articles");
    }

    public override async Task HandleAsync(CreateArticleRequest req, CancellationToken ct)
    {
        await SendOkAsync($"");
    }
}
*/