using LunarLensBackend.Database;
using LunarLensBackend.Entities;

namespace LunarLensBackend.Tests;

public class ApplicationUserTests
{
    [Fact]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        var applicationUser = new ApplicationUser();

        Assert.Empty(applicationUser.Articles);
        Assert.Empty(applicationUser.News);
        Assert.Empty(applicationUser.Events);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(new byte[] { 1, 2, 3, 4 }, "data:image/png;base64,AQIDBA==")]
    public void GetImageAsBase64_ShouldReturnExpectedResult(byte[]? image, string? expected)
    {
        var applicationUser = new ApplicationUser()
        {
            Image = image
        };

        var result = applicationUser.GetImageAsBase64();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetSetMicrosoftId_ShouldSetAndGetValue()
    {
        var user = new ApplicationUser();
        var expectedValue = "some-microsoft-id";
        
        user.MicrosoftId = expectedValue;
        
        Assert.Equal(expectedValue, user.MicrosoftId);
    }

    [Fact]
    public void GetSetImage_ShouldSetAndGetValue()
    {
        
        var user = new ApplicationUser();
        var expectedValue = new byte[] { 0x1, 0x2, 0x3 }; 
        
        user.Image = expectedValue;
        
        Assert.Equal(expectedValue, user.Image);
    }

    [Fact]
    public void GetImageAsBase64_ShouldReturnBase64String_WhenImageIsNotNull()
    {
        
        var user = new ApplicationUser
        {
            Image = new byte[] { 0x1, 0x2, 0x3 }
        };
        
        var result = user.GetImageAsBase64();
        
        Assert.NotNull(result);
        Assert.StartsWith("data:image/png;base64,", result);
    }

    [Fact]
    public void GetImageAsBase64_ShouldReturnNull_WhenImageIsNull()
    {
        
        var user = new ApplicationUser();

        
        var result = user.GetImageAsBase64();
        
        Assert.Null(result);
    }

    [Fact]
    public void AddArticle_ShouldAddArticleToArticlesCollection()
    {
        
        var user = new ApplicationUser();
        var article = new Article();
        
        user.addArticle(article);

        Assert.Contains(article, user.Articles);
    }

    [Fact]
    public void AddNews_ShouldAddNewsToNewsCollection()
    {
        var user = new ApplicationUser();
        var news = new News();
        
        user.addNews(news);
        
        Assert.Contains(news, user.News);
    }

    [Fact]
    public void RemoveArticle_ShouldRemoveArticleFromArticlesCollection()
    {
        var user = new ApplicationUser();
        var article = new Article();
        user.addArticle(article);
        
        user.removeArticle(article);
        
        Assert.DoesNotContain(article, user.Articles);
    }

    [Fact]
    public void RemoveNews_ShouldRemoveNewsFromNewsCollection()
    {
        var user = new ApplicationUser();
        var news = new News();
        user.addNews(news);

        user.removeNews(news);
        
        Assert.DoesNotContain(news, user.News);
    }
}