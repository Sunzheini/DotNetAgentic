﻿﻿using DotNetAgentic.Services;
using NUnit.Framework;

namespace DotNetAgentic.Tests.Services;

[TestFixture]
public class AgentServiceTests
{
    private string? _originalApiKey;
    private ToolRegistry _toolRegistry = null!;
    private IMemoryStore _memoryStore = null!;
    
    [SetUp]
    public void Setup()
    {
        // Save the original API key
        _originalApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        
        // Create a tool registry for tests
        _toolRegistry = new ToolRegistry();
        
        // Create an in-memory store for tests
        _memoryStore = new InMemoryStore();
    }
    
    [TearDown]
    public void TearDown()
    {
        // Restore the original API key
        if (_originalApiKey != null)
        {
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", _originalApiKey);
        }
        else
        {
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);
        }
    }
    
    [Test]
    public void Constructor_WhenApiKeyMissing_ThrowsArgumentException()
    {
        // Arrange
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);
        
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new AgentService(_toolRegistry, _memoryStore));
        
        Assert.That(ex!.Message, Does.Contain("OPENAI_API_KEY"));
    }
    
    [Test]
    public void Constructor_WhenApiKeyProvided_CreatesService()
    {
        // Arrange
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key");
        
        // Act
        var service = new AgentService(_toolRegistry, _memoryStore);
        
        // Assert
        Assert.That(service, Is.Not.Null);
        Assert.That(service, Is.InstanceOf<AgentService>());
    }
    
    [Test]
    public async Task ProcessAsync_WithValidInput_ReturnsString()
    {
        // This test requires a valid API key to actually work
        // Skip if API key is not set or is a test key
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey) || apiKey == "test-key")
        {
            Assert.Ignore("Skipping test - requires valid OPENAI_API_KEY");
        }
        
        // Arrange
        var service = new AgentService(_toolRegistry, _memoryStore);
        
        // Act
        var result = await service.ProcessAsync("Hello AI");
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf<string>());
    }
    
    [Test]
    public void ProcessAsync_WithEmptyInput_ReturnsErrorResponse()
    {
        // This test requires a valid API key to actually work
        // Skip if API key is not set or is a test key
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey) || apiKey == "test-key")
        {
            Assert.Ignore("Skipping test - requires valid OPENAI_API_KEY");
        }
        
        // Arrange
        var service = new AgentService(_toolRegistry, _memoryStore);
        
        // Act
        var result = service.ProcessAsync("").Result;
        
        // Assert - Semantic Kernel returns error message for empty input
        Assert.That(result, Does.Contain("input"));
    }
    
    [Test]
    public void GetAvailableTools_ReturnsRegisteredTools()
    {
        // Arrange
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key");
        var service = new AgentService(_toolRegistry, _memoryStore);
        
        // Act
        var tools = service.GetAvailableTools();
        
        // Assert
        Assert.That(tools, Is.Not.Null);
        Assert.That(tools, Is.Not.Empty);
        Assert.That(tools.Count, Is.GreaterThanOrEqualTo(2));
        Assert.That(tools.Any(t => t.Name == "calculator"), Is.True);
        Assert.That(tools.Any(t => t.Name == "web_search"), Is.True);
    }
    
    [Test]
    public async Task ProcessAsync_WithToolCommand_ExecutesTool()
    {
        // Arrange
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key");
        var service = new AgentService(_toolRegistry, _memoryStore);
        var input = "/tool calculator 5+3";
        
        // Act
        var result = await service.ProcessAsync(input);
        
        // Assert
        Assert.That(result, Is.EqualTo("8"));
    }
    
    [Test]
    public async Task ProcessAsync_WithInvalidToolCommand_ReturnsError()
    {
        // Arrange
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key");
        var service = new AgentService(_toolRegistry, _memoryStore);
        var input = "/tool nonexistent test";
        
        // Act
        var result = await service.ProcessAsync(input);
        
        // Assert
        Assert.That(result, Does.Contain("Error"));
        Assert.That(result, Does.Contain("not found"));
    }
}