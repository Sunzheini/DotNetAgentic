using DotNetAgentic.Services;

namespace DotNetAgentic.Middleware;

public class TelemetryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITelemetryService _telemetryService;
    private readonly IMetricsService _metricsService;
    
    public TelemetryMiddleware(RequestDelegate next, ITelemetryService telemetryService, 
        IMetricsService metricsService)
    {
        _next = next;
        _telemetryService = telemetryService;
        _metricsService = metricsService;
    }
    
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
    
    private async Task<string> ReadResponseBodyAsync(MemoryStream responseBody)
    {
        responseBody.Position = 0;
        using var reader = new StreamReader(responseBody, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        responseBody.Position = 0;
        
        return body;
    }
    
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