using DotNetAgentic.Tools;
using NUnit.Framework;

namespace DotNetAgentic.Tests.Tools;

[TestFixture]
public class CalculatorToolTests
{
    private CalculatorTool _tool = null!;
    
    [SetUp]
    public void Setup()
    {
        _tool = new CalculatorTool();
    }
    
    [Test]
    public void Name_ReturnsCorrectValue()
    {
        // Act & Assert
        Assert.That(_tool.Name, Is.EqualTo("calculator"));
    }
    
    [Test]
    public void Description_ReturnsCorrectValue()
    {
        // Act & Assert
        Assert.That(_tool.Description, Is.EqualTo("Performs mathematical calculations"));
    }
    
    [Test]
    public async Task ExecuteAsync_WithAddition_ReturnsCorrectResult()
    {
        // Arrange
        var input = "2+2";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert
        Assert.That(result, Is.EqualTo("4"));
    }
    
    [Test]
    public async Task ExecuteAsync_WithSubtraction_ReturnsCorrectResult()
    {
        // Arrange
        var input = "10-3";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert
        Assert.That(result, Is.EqualTo("7"));
    }
    
    [Test]
    public async Task ExecuteAsync_WithMultiplication_ReturnsCorrectResult()
    {
        // Arrange
        var input = "5*6";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert
        Assert.That(result, Is.EqualTo("30"));
    }
    
    [Test]
    public async Task ExecuteAsync_WithDivision_ReturnsCorrectResult()
    {
        // Arrange
        var input = "20/4";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert
        Assert.That(result, Is.EqualTo("5"));
    }
    
    [Test]
    public async Task ExecuteAsync_WithComplexExpression_ReturnsCorrectResult()
    {
        // Arrange
        var input = "(10+5)*2";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert
        Assert.That(result, Is.EqualTo("30"));
    }
    
    [Test]
    public async Task ExecuteAsync_WithInvalidExpression_ReturnsError()
    {
        // Arrange
        var input = "not a number";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert
        Assert.That(result, Does.StartWith("Error:"));
    }
    
    [Test]
    public async Task ExecuteAsync_WithDivisionByZero_HandlesCorrectly()
    {
        // Arrange
        var input = "10/0";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert - DataTable.Compute returns infinity symbol (∞) for division by zero
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
    }
    
    [Test]
    public async Task ExecuteAsync_WithDecimalNumbers_ReturnsCorrectResult()
    {
        // Arrange
        var input = "3.5+2.5";
        
        // Act
        var result = await _tool.ExecuteAsync(input);
        
        // Assert - DataTable.Compute returns "6.0" for decimal operations
        Assert.That(result, Is.EqualTo("6.0"));
    }
}
