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

        try
        {
            switch (req.Type)
            {
                case ContentType.News:
                    var newsStory = _context.News.FirstOrDefault(news => news.Id == req.ContentId);
                    
                    if (newsStory == null)
                    {
                        await SendAsync(new { Message = "News story not found. Check id." }, StatusCodes.Status400BadRequest);
                        return;
                    }

                    newsStory.Status = req.Status;
                    await _context.SaveChangesAsync();
                    break;
                
                case ContentType.Article:
                    var articleStory = _context.Articles.FirstOrDefault(article => article.Id == req.ContentId);
                    
                    if (articleStory == null)
                    {
                        await SendAsync(new { Message = "Article not found. Check id." }, StatusCodes.Status400BadRequest);
                        return;
                    }

                    articleStory.Status = req.Status;
                    await _context.SaveChangesAsync();
                    break;

                case ContentType.Event:
                    var eventStory = _context.Events.FirstOrDefault(e => e.Id == req.ContentId);
                    
                    if (eventStory == null)
                    {
                        await SendAsync(new { Message = "Event not found. Check id." }, StatusCodes.Status400BadRequest);
                        return;
                    }

                    eventStory.Status = req.Status;
                    await _context.SaveChangesAsync();
                    break;
            }
            
            await SendOkAsync("Content status changed successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await SendAsync(new { Message = "An error occurred while changing the content status." }, StatusCodes.Status500InternalServerError);
        }
    }

}