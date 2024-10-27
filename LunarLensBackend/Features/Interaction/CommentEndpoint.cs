using System.Security.Claims;
using System.Text.Json.Serialization;
using FastEndpoints;
using LunarLensBackend.Database;
using LunarLensBackend.Entities;
using LunarLensBackend.Entities.Enums;
using LunarLensBackend.Utility;
using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Features.Interaction;

public class CreateCommentEndpoint : Endpoint<CreateCommentRequest>
{
    private readonly Context _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateCommentEndpoint(Context context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public override void Configure()
    {
        Post("/comment/create");
        Policies("BasicUserOnly");
    }

    public override async Task HandleAsync(CreateCommentRequest req, CancellationToken ct)
    {
        if (!Enum.IsDefined(typeof(ContentType), req.Type))
        {
            await SendAsync(new { Message = "Invalid content type provided." }, StatusCodes.Status400BadRequest);
            return;
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            await SendAsync(new { Message = "User ID not found." }, StatusCodes.Status401Unauthorized);
            return;
        }
        
        var comment = new Comment
        {
            Text = req.Text,
            UserId = userId
        };
        
        _context.Comments.Add(comment);

        try
        {
            switch (req.Type)
            {
                case ContentType.News:
                    var news = _context.News.FirstOrDefault(n => n.Id == req.ContentId);
                    if (news == null)
                    {
                        await SendAsync(new { Message = "News not found." }, StatusCodes.Status404NotFound);
                        return;
                    }
                    news.Comments.Add(comment);
                    break;
                
                case ContentType.Article:
                    var article = _context.Articles.FirstOrDefault(n => n.Id == req.ContentId);
                    if (article == null)
                    {
                        await SendAsync(new { Message = "Article not found." }, StatusCodes.Status404NotFound);
                        return;
                    }
                    article.Comments.Add(comment);
                    break;
                
                case ContentType.Event:
                    var eEvent = _context.Events.FirstOrDefault(n => n.Id == req.ContentId);
                    if (eEvent == null)
                    {
                        await SendAsync(new { Message = "Event not found." }, StatusCodes.Status404NotFound);
                        return;
                    }
                    eEvent.Comments.Add(comment);
                    break;
            }
            
            await _context.SaveChangesAsync();
            await SendAsync(new { Message = "Comment created successfully." }, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await SendAsync(new { Message = "An error occurred while creating the comment." }, StatusCodes.Status500InternalServerError);
        }
    }
}
public class EditCommentEndpoint : Endpoint<EditCommentRequest>
{
    private readonly Context _context;

    public EditCommentEndpoint(Context context)
    {
        _context = context;
    }
    
    public override void Configure()
    {
        Put("/comment/edit");
        Policies("BasicUserOnly");
    }

    public override async Task HandleAsync(EditCommentRequest req, CancellationToken ct)
    {
        try
        {
            var comment = await _context.Comments.FindAsync(req.Id);
            if (comment == null)
            {
                await SendAsync(new { Message = "Comment not found." }, StatusCodes.Status404NotFound);
                return;
            }
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (comment.UserId != userId)
            {
                await SendAsync(new { Message = "Unauthorized to delete this comment." }, StatusCodes.Status403Forbidden);
                return;
            }
        
            comment.Text = req.Text;
            comment.IsEdited = true;
        
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        
            await SendAsync(new { Message = "Comment edited successfully." }, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await SendAsync(new { Message = "An error occurred while editing the comment." }, StatusCodes.Status500InternalServerError);
        }
    }
}
public class DeleteCommentEndpoint : EndpointWithoutRequest
{
    private readonly Context _context;

    public DeleteCommentEndpoint(Context context)
    {
        _context = context;
    }
    
    public override void Configure()
    {
        Delete("/comment/delete/{id}");
        Policies("BasicUserOnly");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            int commentId = Route<int>("id");
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                await SendAsync(new { Message = "Comment not found." }, StatusCodes.Status404NotFound);
                return;
            }
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (comment.UserId != userId)
            {
                await SendAsync(new { Message = "Unauthorized to delete this comment." }, StatusCodes.Status403Forbidden);
                return;
            }
            
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            
            await SendNoContentAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await SendAsync(new { Message = "An error occurred while deleting the comment." }, StatusCodes.Status500InternalServerError);
        }
    }
}

public class CreateCommentRequest
{
    public string Text { get; set; }
    public int ContentId { get; set; }
    [JsonConverter(typeof(SafeEnumConverter<ContentType>))]
    [EnumValidation(typeof(ContentType))]
    public ContentType Type { get; set; }
}

public class EditCommentRequest
{
    public int Id { get; set; }
    public string Text { get; set; }
}

