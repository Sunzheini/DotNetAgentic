using DotNetAgentic.Services;
using DotNetAgentic.Services.Interfaces;

namespace DotNetAgentic.Middleware;

/// <summary>
/// ASP.NET Core middleware for logging telemetry and metrics for each HTTP request.
/// </summary>
public class TelemetryMiddleware
{
    /// <summary>
    /// The next middleware in the pipeline.
    /// </summary>
    private readonly RequestDelegate _next;
    
    /// <summary>
    /// Telemetry service for logging requests.
    /// </summary>
    private readonly ITelemetryService _telemetryService;
    
    /// <summary>
    /// The metrics service for recording request metrics.
    /// </summary>
    private readonly IMetricsService _metricsService;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public TelemetryMiddleware(RequestDelegate next, ITelemetryService telemetryService, 
        IMetricsService metricsService)
    {
        _next = next;
        _telemetryService = telemetryService;
        _metricsService = metricsService;
    }
    
    /// <summary>
    /// Invoke the middleware to log telemetry and metrics for the HTTP request.
    /// </summary>
    /// <param name="context">
    /// The HTTP context of the current request.
    /// </param>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var sessionId = context.Request.Headers["X-Session-Id"].FirstOrDefault() ?? "anonymous";
        
        try
        {
            // Read request body
            string requestBody = await ReadRequestBodyAsync(context.Request);
            
            // Store original response body stream
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            
            await _next(context);
            
            stopwatch.Stop();
            
            // Read response body
            var responseBodyText = await ReadResponseBodyAsync(responseBody);
            
            // Log successful request
            await _telemetryService.LogRequestAsync(
                endpoint: context.Request.Path,
                sessionId: sessionId,
                operation: context.Request.Method,
                input: requestBody,
                output: responseBodyText,
                durationMs: stopwatch.ElapsedMilliseconds
            );
            
            await _metricsService.RecordRequestAsync(
                endpoint: context.Request.Path,
                toolName: ExtractToolName(requestBody),
                durationMs: stopwatch.ElapsedMilliseconds,
                success: context.Response.StatusCode < 400
            );
            
            // Copy response back to original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            await _telemetryService.LogRequestAsync(
                endpoint: context.Request.Path,
                sessionId: sessionId,
                operation: context.Request.Method,
                input: "",
                output: "",
                durationMs: stopwatch.ElapsedMilliseconds,
                error: ex.Message
            );
            
            await _metricsService.RecordRequestAsync(
                endpoint: context.Request.Path,
                toolName: "",
                durationMs: stopwatch.ElapsedMilliseconds,
                success: false
            );
            
            throw;
        }
    }
    
    /// <summary>
    /// Reads the body of the HTTP request as a string.
    /// </summary>
    /// <param name="request">
    /// The HTTP request to read the body from.
    /// </param>
    /// <returns>
    /// Returns the request body as a string.
    /// </returns>
    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        if (!request.Body.CanRead || request.ContentLength == 0)
            return string.Empty;
        
        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        
        return body;
    }
    
    /// <summary>
    /// Reads the body of the HTTP response from a MemoryStream as a string.
    /// </summary>
    /// <param name="responseBody">
    /// Response body stream to read from.
    /// </param>
    /// <returns>
    /// Returns the response body as a string.
    /// </returns>
    private async Task<string> ReadResponseBodyAsync(MemoryStream responseBody)
    {
        responseBody.Position = 0;
        using var reader = new StreamReader(responseBody, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        responseBody.Position = 0;
        
        return body;
    }
    
    /// <summary>
    /// Extracts the tool name from the request body if present.
    /// </summary>
    /// <param name="requestBody">
    /// Request body to extract tool name from.
    /// </param>
    /// <returns>
    /// The extracted tool name, or an empty string if not found.
    /// </returns>
    private string ExtractToolName(string requestBody)
    {
        if (string.IsNullOrEmpty(requestBody) || !requestBody.Contains("toolName"))
            return string.Empty;
        
        try
        {
            var toolMatch = System.Text.RegularExpressions.Regex.Match(
                requestBody, "\"toolName\"\\s*:\\s*\"([^\"]+)\"");
            return toolMatch.Success ? toolMatch.Groups[1].Value : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}