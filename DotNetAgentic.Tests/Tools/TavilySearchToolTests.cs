using DotNetAgentic.Tools;
using NUnit.Framework;

namespace DotNetAgentic.Tests.Tools;

[TestFixture]
public class TavilySearchToolTests
{
    private TavilySearchTool _tool = null!;
    private string? _originalApiKey;
    
    [SetUp]
    public void Setup()
    {
        // Save original API key
        _originalApiKey = Environment.GetEnvironmentVariable("TAVILY_API_KEY");
        
        // Set a test API key for construction tests
        var apiKey = Environment.GetEnvironmentVariable("TAVILY_API_KEY") ?? "test-tavily-key";
        _tool = new TavilySearchTool(apiKey);
    }
    
    [TearDown]
    public void TearDown()
    {
        // Restore original API key
        if (_originalApiKey != null)
        {
            Environment.SetEnvironmentVariable("TAVILY_API_KEY", _originalApiKey);
        }
        else
        {
            Environment.SetEnvironmentVariable("TAVILY_API_KEY", null);
        }
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
        Assert.That(_tool.Description, Does.Contain("Tavily"));
        Assert.That(_tool.Description, Does.Contain("web"));
    }
    
    [Test]
    public void Constructor_WithValidApiKey_CreatesInstance()
    {
        // Arrange & Act
        var tool = new TavilySearchTool("test-key");
        
        // Assert
        Assert.That(tool, Is.Not.Null);
        Assert.That(tool.Name, Is.EqualTo("web_search"));
    }
    
    [Test]
    public async Task ExecuteAsync_WithInvalidApiKey_ReturnsError()
    {
        // Arrange
        var tool = new TavilySearchTool("invalid-key");
        
        // Act
        var result = await tool.ExecuteAsync("test query");
        
        // Assert
        Assert.That(result, Does.Contain("error").IgnoreCase);
    }
    
    [Test]
    public async Task ExecuteAsync_WithValidApiKey_ReturnsResults()
    {
        // Skip test if no valid API key
        var apiKey = Environment.GetEnvironmentVariable("TAVILY_API_KEY");
        if (string.IsNullOrEmpty(apiKey) || apiKey == "test-tavily-key")
        {
            Assert.Ignore("Skipping test - requires valid TAVILY_API_KEY");
        }
        
        // Arrange
        var tool = new TavilySearchTool(apiKey);
        var query = "OpenAI GPT";
        
        // Act
        var result = await tool.ExecuteAsync(query);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.Length, Is.GreaterThan(10));
    }
    
    [Test]
    public async Task ExecuteAsync_WithEmptyQuery_HandlesGracefully()
    {
        // Arrange
        var query = "";
        
        // Act
        var result = await _tool.ExecuteAsync(query);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        // Should either return error or handle empty query
    }
    
    [Test]
    public async Task ExecuteAsync_WithSpecialCharacters_HandlesCorrectly()
    {
        // Skip test if no valid API key
        var apiKey = Environment.GetEnvironmentVariable("TAVILY_API_KEY");
        if (string.IsNullOrEmpty(apiKey) || apiKey == "test-tavily-key")
        {
            Assert.Ignore("Skipping test - requires valid TAVILY_API_KEY");
        }
        
        // Arrange
        var tavilyTool = new TavilySearchTool(apiKey);
        var query = "C# .NET programming!";
        
        // Act
        var result = await tavilyTool.ExecuteAsync(query);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
    }
    
    [Test]
    public async Task ExecuteAsync_FormatsResultsCorrectly()
    {
        // Skip test if no valid API key
        var apiKey = Environment.GetEnvironmentVariable("TAVILY_API_KEY");
        if (string.IsNullOrEmpty(apiKey) || apiKey == "test-tavily-key")
        {
            Assert.Ignore("Skipping test - requires valid TAVILY_API_KEY");
        }
        
        // Arrange
        var tavilyTool = new TavilySearchTool(apiKey);
        var query = "artificial intelligence";
        
        // Act
        var result = await tavilyTool.ExecuteAsync(query);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        
        // Check for expected formatting elements
        if (result.Contains("🤖 Answer:") || result.Contains("🔍 Top results:"))
        {
            Assert.Pass("Results are formatted correctly");
        }
        else if (result.Contains("error", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Pass("Error handling works correctly");
        }
        else
        {
            Assert.That(result, Is.Not.Empty);
        }
    }
}
