using LunarLensBackend.Entities;

namespace LunarLensBackend.Tests;

public class CategoryTests
{
    [Fact]
    public void Constructor_Creates_Category_Without_Parameters()
    {
        var category = new Category();
        Assert.NotNull(category);
    }

    [Theory]
    [InlineData("category name", "category description")]
    public void Constructor_Creates_Category_With_Parameters(string categoryName, string categoryDescription)
    {
        var category = new Category(categoryName, categoryDescription);
        Assert.Equal(categoryName, category.Name);
        Assert.Equal(categoryDescription, category.Description);
    }
    
    [Fact]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        var category = new Category();
        
        Assert.Empty(category.Articles);
        Assert.Empty(category.News);
        Assert.Empty(category.Events);
    }
}