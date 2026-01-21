using DotNetAgentic.Tools;
using NUnit.Framework;

namespace DotNetAgentic.Tests.Tools;

[TestFixture]
public class WebSearchToolTests
{
    private WebSearchTool _tool = null!;
    
    [SetUp]
    public void Setup()
    {
        _tool = new WebSearchTool();
    }
    
    [Test]
    public void Name_ReturnsCorrectValue()
    {
        // Act & Assert
        Assert.That(_tool.Name, Is.EqualTo("web_search"));
    }
    
    [Test]
    public void Description_ReturnsCorrectValue()
    {
        // Act & Assert
        Assert.That(_tool.Description, Is.EqualTo("Searches the web for information"));
    }
    
    [Test]
    public async Task ExecuteAsync_WithValidQuery_ReturnsSearchResults()
    {
        // Arrange
        var input = "AI agentic patterns";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Does.Contain("AI agentic patterns"));
        Assert.That(result, Does.Contain("Search results"));
    }
    
    [Test]
    public async Task ExecuteAsync_WithDifferentQuery_IncludesQueryInResult()
    {
        // Arrange
        var input = "machine learning algorithms";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert
        Assert.That(result, Does.Contain("machine learning algorithms"));
    }
    
    [Test]
    public async Task ExecuteAsync_ReturnsMultipleResults()
    {
        // Arrange
        var input = "test query";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert - Mock returns multiple numbered results
        Assert.That(result, Does.Contain("1."));
        Assert.That(result, Does.Contain("2."));
    }
    
    [Test]
    public async Task ExecuteAsync_WithEmptyQuery_StillReturnsResult()
    {
        // Arrange
        var input = "";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Does.Contain("Search results"));
    }
    
    [Test]
    public async Task ExecuteAsync_WithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var input = "C# .NET programming!";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert
        Assert.That(result, Does.Contain(input));
    }
}
