using DotNetAgentic.Services;
using DotNetAgentic.Tools;
using DotNetAgentic.Tools.Interfaces;
using NUnit.Framework;

namespace DotNetAgentic.Tests.Services;

[TestFixture]
public class ToolRegistryTests
{
    private ToolRegistry _registry = null!;
    
    [SetUp]
    public void Setup()
    {
        _registry = new ToolRegistry();
    }
    
    [Test]
    public void Constructor_RegistersDefaultTools()
    {
        // Act
        var tools = _registry.GetAllTools();
        
        // Assert
        Assert.That(tools, Is.Not.Null);
        Assert.That(tools.Count, Is.GreaterThanOrEqualTo(2));
        Assert.That(tools.Any(t => t.Name == "calculator"), Is.True);
        Assert.That(tools.Any(t => t.Name == "web_search"), Is.True);
    }
    
    [Test]
    public void HasTool_WithRegisteredTool_ReturnsTrue()
    {
        // Act & Assert
        Assert.That(_registry.HasTool("calculator"), Is.True);
        Assert.That(_registry.HasTool("web_search"), Is.True);
    }
    
    [Test]
    public void HasTool_WithUnregisteredTool_ReturnsFalse()
    {
        // Act & Assert
        Assert.That(_registry.HasTool("nonexistent_tool"), Is.False);
    }
    
    [Test]
    public void RegisterTool_AddsNewTool()
    {
        // Arrange
        var mockTool = new MockTool();
        
        // Act
        _registry.RegisterTool(mockTool);
        
        // Assert
        Assert.That(_registry.HasTool("mock_tool"), Is.True);
    }
    
    [Test]
    public async Task ExecuteToolAsync_WithCalculator_ReturnsResult()
    {
        // Arrange
        var input = "5+3";
        
        // Act
        var result = await _registry.ExecuteToolAsync("calculator", input);
        
        // Assert
        Assert.That(result, Is.EqualTo("8"));
    }
    
    [Test]
    public async Task ExecuteToolAsync_WithWebSearch_ReturnsResult()
    {
        // Arrange
        var input = "AI agentic patterns";
        
        // Act
        var result = await _registry.ExecuteToolAsync("web_search", input);
        
        // Assert
        Assert.That(result, Does.Contain("AI agentic patterns"));
    }
    
    [Test]
    public async Task ExecuteToolAsync_WithUnregisteredTool_ReturnsError()
    {
        // Arrange
        var toolName = "nonexistent_tool";
        var input = "test";
        
        // Act
        var result = await _registry.ExecuteToolAsync(toolName, input);
        
        // Assert
        Assert.That(result, Does.StartWith("Error:"));
        Assert.That(result, Does.Contain("not found"));
    }
    
    [Test]
    public void GetAllTools_ReturnsAllRegisteredTools()
    {
        // Act
        var tools = _registry.GetAllTools();
        
        // Assert
        Assert.That(tools, Is.Not.Null);
        Assert.That(tools, Is.Not.Empty);
        Assert.That(tools.Count, Is.GreaterThanOrEqualTo(2));
    }
    
    [Test]
    public void GetToolsDescription_ReturnsFormattedDescription()
    {
        // Act
        var description = _registry.GetToolsDescription();
        
        // Assert
        Assert.That(description, Is.Not.Null);
        Assert.That(description, Does.Contain("calculator"));
        Assert.That(description, Does.Contain("web_search"));
        Assert.That(description, Does.Contain("Performs mathematical calculations"));
        Assert.That(description, Does.Contain("Searches the web for information"));
    }
    
    [Test]
    public void RegisterTool_WithSameName_ReplacesExistingTool()
    {
        // Arrange
        var originalCount = _registry.GetAllTools().Count;
        var newCalculatorTool = new CalculatorTool();
        
        // Act
        _registry.RegisterTool(newCalculatorTool);
        
        // Assert
        Assert.That(_registry.GetAllTools().Count, Is.EqualTo(originalCount));
        Assert.That(_registry.HasTool("calculator"), Is.True);
    }
    
    // Mock tool for testing
    private class MockTool : ITool
    {
        public string Name => "mock_tool";
        public string Description => "A mock tool for testing";
        
        public Task<string> ExecuteAsync(string input)
        {
            return Task.FromResult($"Mock result for: {input}");
        }
    }
}
