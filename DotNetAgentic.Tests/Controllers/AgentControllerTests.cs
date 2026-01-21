using DotNetAgentic.Controllers;
using DotNetAgentic.Models;
using DotNetAgentic.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace DotNetAgentic.Tests.Controllers;

[TestFixture]
public class AgentControllerTests
{
    private Mock<IAgentService> _mockAgentService = null!;
    private AgentController _controller = null!;
    
    [SetUp]
    public void Setup()
    {
        _mockAgentService = new Mock<IAgentService>();
        _controller = new AgentController(_mockAgentService.Object);
    }
    
    [Test]
    public async Task Chat_ValidRequest_ReturnsOkWithAgentResponse()
    {
        // Arrange
        _mockAgentService.Setup(x => x.ProcessAsync("Hello"))
            .ReturnsAsync("Mocked AI response");
        
        var request = new AgentRequest 
        { 
            Message = "Hello", 
            SessionId = "test-session" 
        };
        
        // Act
        var result = await _controller.Chat(request);
        
        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        
        var okResult = (OkObjectResult)result.Result!;
        var response = (AgentResponse)okResult.Value!;
        
        Assert.That(response.Content, Is.EqualTo("Mocked AI response"));
        Assert.That(response.ToolCalls, Is.Empty);
    }
    
    [Test]
    public async Task Chat_ServiceThrowsException_ReturnsBadRequest()
    {
        // Arrange
        _mockAgentService.Setup(x => x.ProcessAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("API error"));
        
        var request = new AgentRequest 
        { 
            Message = "Test", 
            SessionId = "test" 
        };
        
        // Act
        var result = await _controller.Chat(request);
        
        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }
    
    [Test]
    public void HealthCheck_ReturnsOkWithStatus()
    {
        // Act
        var result = _controller.HealthCheck();
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.Not.Null);
    }
}