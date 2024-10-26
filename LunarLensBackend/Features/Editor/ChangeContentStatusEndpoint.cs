using FastEndpoints;
using LunarLensBackend.Database;
using LunarLensBackend.DTOs;
using LunarLensBackend.Entities;
using LunarLensBackend.Entities.Enums;
using LunarLensBackend.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace LunarLensBackend.Features.Editor;

public class ChangeContentStatusEndpoint : Endpoint<ChangeContentStatusRequest>
{
    private readonly Context _context;

    public ChangeContentStatusEndpoint(Context context)
    {
        _context = context;
    }
    
    public override void Configure()
    {
        Put("/editor/change-content-status");
        Policies("EditorOnly");
    }
    
    public override async Task HandleAsync(ChangeContentStatusRequest req, CancellationToken ct)
    {
        if (!Enum.IsDefined(typeof(ContentStatus), req.Status))
        {
            await SendAsync(new { Message = "Invalid status provided." }, StatusCodes.Status400BadRequest);
            return;
        }

        Console.WriteLine("------------------------------------------------------");
        Console.WriteLine($"Status: {req.Status}");
        Console.WriteLine($"Id: {req.ContentId}");
        Console.WriteLine($"Type: {req.Type}");
        Console.WriteLine("------------------------------------------------------");

        try
        {
            /*ContentBase? content = await _context.ContentBases
                .Where(c => c.Id == req.ContentId && c.Type == req.Type)
                .FirstOrDefaultAsync();

            if (content == null)
            {
                await SendAsync(new { Message = $"{req.Type} content not found. Check id." }, StatusCodes.Status400BadRequest);
                return;
            }

            content.Status = req.Status;
            await _context.SaveChangesAsync();

            await SendOkAsync("Content status changed successfully!");*/
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await SendAsync(new { Message = "An error occurred while changing the content status." }, StatusCodes.Status500InternalServerError);
        }
    }

}