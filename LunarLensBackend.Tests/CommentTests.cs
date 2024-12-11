using Azure.Core.GeoJson;
using LunarLensBackend.Entities;


namespace LunarLensBackend.Tests;

public class CommentTests
{
    [Fact]
    public void Constructor_Creates_Category_Without_Parameters()
    {
        var comment = new Comment();
        Assert.NotNull(comment);
    }

    [Theory]
    [InlineData("text", "randomuuid", null, null, 1)]
    public void Constructor_Creates_Category_With_Parameters(string text, string userId, int? articleId, int? eventId, int? newsId)
    {
        var comment = new Comment(text, userId, articleId, eventId, newsId);
        Assert.NotNull(comment.Text);
        Assert.NotNull(comment.UserId);
        Assert.NotNull(comment.NewsId);
    }
    
    [Fact]
    public void DefaultValues_ShouldBeSetCorrectly()
    {

    }
}