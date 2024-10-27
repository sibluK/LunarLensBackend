using System.Text.Json.Serialization;
using FastEndpoints;
using LunarLensBackend.Database;
using LunarLensBackend.DTOs;
using LunarLensBackend.Entities.Enums;
using LunarLensBackend.Utility;

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
                    if (req.Status == ContentStatus.Published)
                    {
                        newsStory.PublishedDate = DateTime.UtcNow;
                    }
                    await _context.SaveChangesAsync();
                    break;
                
                case ContentType.Article:
                    var article = _context.Articles.FirstOrDefault(article => article.Id == req.ContentId);
                    
                    if (article == null)
                    {
                        await SendAsync(new { Message = "Article not found. Check id." }, StatusCodes.Status400BadRequest);
                        return;
                    }

                    article.Status = req.Status;
                    if (req.Status == ContentStatus.Published)
                    {
                        article.PublishedDate = DateTime.UtcNow;
                    }
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

public class ChangeContentStatusRequest
{
    public required int ContentId { get; set; }
    
    [JsonConverter(typeof(SafeEnumConverter<ContentType>))]
    [EnumValidation(typeof(ContentType))]
    public required ContentType Type { get; set; }

    [JsonConverter(typeof(SafeEnumConverter<ContentStatus>))]
    [EnumValidation(typeof(ContentStatus))]
    public required ContentStatus Status { get; set; }
}