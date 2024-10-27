using FastEndpoints;
using LunarLensBackend.Database;
using LunarLensBackend.DTOs;
using LunarLensBackend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LunarLensBackend.Features.Writer;

public class DraftEventEndpoint : Endpoint<DraftEventRequest>
{
    private readonly Context _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DraftEventEndpoint(Context context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/writer/draft-event");
        Policies("WriterOnly");
    }

    public override async Task HandleAsync(DraftEventRequest req, CancellationToken ct)
    {
        try
        {
            Event eEvent = new Event
            {
                Title = req.Title,
                Summary = req.Summary,
                Image = req.Image,
                StartDate = req.StartDate,
                Location = req.Location,
                Organizer = req.Organizer
            };

            if (req.ContentSections.Count > 0)
            {
                foreach (var section in req.ContentSections)
                {
                    ContentSection contentSection = new ContentSection
                    {
                        Text = section.Text,
                        Image = section.Image
                    };
                    eEvent.ContentSections.Add(contentSection);
                }
            }
            else
            {
                await SendAsync(new { Message = "No content sections found!" }, StatusCodes.Status400BadRequest);
                return;
            }

            if (req.Writers.Count > 0)
            {
                foreach (var w in req.Writers)
                {
                    var user = await _userManager.FindByEmailAsync(w.Email);

                    if (user != null)
                    {
                        eEvent.Writers.Add(user);
                    }
                    else
                    {
                        await SendAsync(new { Message = "No user found with that email!" }, StatusCodes.Status400BadRequest);
                        return;
                    }
                }
            }
            else
            {
                await SendAsync(new { Message = "No writer found!" }, StatusCodes.Status400BadRequest);
                return;
            }
            
            if (req.Categories.Count > 0)
            {
                foreach (var cat in req.Categories)
                {
                    var category = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name == cat.Name);
                    
                    if (category == null)
                    {
                        await SendAsync(new { Message = "Category not found!" }, StatusCodes.Status400BadRequest);
                        return;
                    }
                    eEvent.Categories.Add(category);
                }
            } 
            else
            {
                await SendAsync(new { Message = "No categories found!" }, StatusCodes.Status400BadRequest);
                return;
            }

            _context.Events.Add(eEvent); 
            await _context.SaveChangesAsync();
            await SendAsync(new { Message = "Draft event added!" }, StatusCodes.Status201Created);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await SendAsync(new { Message = "An error occurred while drafting the article" }, StatusCodes.Status500InternalServerError);
        }
    }
}