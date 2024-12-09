using LunarLensBackend.Entities;
using LunarLensBackend.Entities.Enums;

namespace LunarLensBackend.Tests;

public class ArticleTests
{
    [Fact]
    public void SettingTitle_ShouldGenerateSlug()
    {
        // Arrange
        var article = new Article();
        var title = "An Example Title!";

        // Act
        article.Title = title;

        // Assert
        Assert.Equal("an-example-title", article.Slug);
    }

    [Theory]
    [InlineData("Simple Title", "simple-title")]
    [InlineData("Complex @Title#", "complex-title")]
    [InlineData("Title with Numbers 123", "title-with-numbers-123")]
    [InlineData(null, null)]
    public void Title_ShouldGenerateExpectedSlug(string inputTitle, string expectedSlug)
    {

        var article = new Article();


        article.Title = inputTitle;


        Assert.Equal(expectedSlug, article.Slug);
    }

    [Fact]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        // Arrange & Act
        var article = new Article();

        // Assert
        Assert.Equal(0, article.Views);
        Assert.Equal(0, article.Likes);
        Assert.Equal(0, article.Dislikes);
        Assert.NotNull(article.Comments);
        Assert.Empty(article.Comments);
        Assert.NotNull(article.ContentSections);
        Assert.Empty(article.ContentSections);
        Assert.NotNull(article.Writers);
        Assert.Empty(article.Writers);
        Assert.NotNull(article.Categories);
        Assert.Empty(article.Categories);
        Assert.Equal(ContentStatus.Drafted, article.Status);
    }
}