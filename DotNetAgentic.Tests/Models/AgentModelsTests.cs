using DotNetAgentic.Models;
using NUnit.Framework;

namespace DotNetAgentic.Tests.Models;

[TestFixture]
public class AgentModelsTests
{
    [Test]
    public void AgentRequest_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var request = new AgentRequest();
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(request.Message, Is.EqualTo(string.Empty));
            Assert.That(request.SessionId, Is.Null);
        });
    }
    
    [Test]
    public void AgentRequest_Properties_CanBeSet()
    {
        // Arrange & Act
        var request = new AgentRequest
        {
            Message = "Test message",
            SessionId = "session-123"
        };
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(request.Message, Is.EqualTo("Test message"));
            Assert.That(request.SessionId, Is.EqualTo("session-123"));
        });
    }
    
    [Test]
    public void AgentResponse_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var response = new AgentResponse();
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Content, Is.EqualTo(string.Empty));
            Assert.That(response.ToolCalls, Is.Empty);
            Assert.That(response.Reasoning, Is.Null);
        });
    }
    
    [Test]
    public void ToolCall_Properties_WorkCorrectly()
    {
        // Arrange & Act
        var toolCall = new ToolCall
        {
            ToolName = "Calculator",
            Input = "2+2",
            Output = "4"
        };
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(toolCall.ToolName, Is.EqualTo("Calculator"));
            Assert.That(toolCall.Input, Is.EqualTo("2+2"));
            Assert.That(toolCall.Output, Is.EqualTo("4"));
        });
    }
}