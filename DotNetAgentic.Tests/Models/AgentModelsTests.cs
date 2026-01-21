﻿using DotNetAgentic.Models;
using NUnit.Framework;
using System.Text.Json;

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
    public void AgentRequest_CanBeSerializedToJson()
    {
        // Arrange
        var request = new AgentRequest
        {
            Message = "Hello AI",
            SessionId = "test-session"
        };
        
        // Act
        var json = JsonSerializer.Serialize(request);
        
        // Assert
        Assert.That(json, Does.Contain("Hello AI"));
        Assert.That(json, Does.Contain("test-session"));
    }
    
    [Test]
    public void AgentRequest_CanBeDeserializedFromJson()
    {
        // Arrange
        var json = """{"Message":"Test message","SessionId":"session-123"}""";
        
        // Act
        var request = JsonSerializer.Deserialize<AgentRequest>(json);
        
        // Assert
        Assert.That(request, Is.Not.Null);
        Assert.That(request!.Message, Is.EqualTo("Test message"));
        Assert.That(request.SessionId, Is.EqualTo("session-123"));
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
    public void AgentResponse_WithAllProperties_CanBeCreated()
    {
        // Arrange & Act
        var response = new AgentResponse
        {
            Content = "AI response text",
            ToolCalls = new List<ToolCall>
            {
                new() { ToolName = "calculator", Input = "2+2", Output = "4" }
            },
            Reasoning = "Used calculator to compute result"
        };
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Content, Is.EqualTo("AI response text"));
            Assert.That(response.ToolCalls, Has.Count.EqualTo(1));
            Assert.That(response.Reasoning, Is.EqualTo("Used calculator to compute result"));
        });
    }
    
    [Test]
    public void AgentResponse_CanBeSerializedToJson()
    {
        // Arrange
        var response = new AgentResponse
        {
            Content = "Test response",
            ToolCalls = new List<ToolCall>(),
            Reasoning = "Test reasoning"
        };
        
        // Act
        var json = JsonSerializer.Serialize(response);
        
        // Assert
        Assert.That(json, Does.Contain("Test response"));
        Assert.That(json, Does.Contain("Test reasoning"));
    }
    
    [Test]
    public void AgentResponse_WithMultipleToolCalls_CanBeCreated()
    {
        // Arrange & Act
        var response = new AgentResponse
        {
            Content = "Used multiple tools",
            ToolCalls = new List<ToolCall>
            {
                new() { ToolName = "calculator", Input = "5+5", Output = "10" },
                new() { ToolName = "web_search", Input = "AI", Output = "Results" }
            }
        };
        
        // Assert
        Assert.That(response.ToolCalls, Has.Count.EqualTo(2));
        Assert.That(response.ToolCalls[0].ToolName, Is.EqualTo("calculator"));
        Assert.That(response.ToolCalls[1].ToolName, Is.EqualTo("web_search"));
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
    
    [Test]
    public void ToolCall_DefaultValues_AreEmpty()
    {
        // Arrange & Act
        var toolCall = new ToolCall();
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(toolCall.ToolName, Is.EqualTo(string.Empty));
            Assert.That(toolCall.Input, Is.EqualTo(string.Empty));
            Assert.That(toolCall.Output, Is.EqualTo(string.Empty));
        });
    }
    
    [Test]
    public void ToolCall_CanBeSerializedToJson()
    {
        // Arrange
        var toolCall = new ToolCall
        {
            ToolName = "web_search",
            Input = "AI agentic patterns",
            Output = "Search results..."
        };
        
        // Act
        var json = JsonSerializer.Serialize(toolCall);
        
        // Assert
        Assert.That(json, Does.Contain("web_search"));
        Assert.That(json, Does.Contain("AI agentic patterns"));
    }
}