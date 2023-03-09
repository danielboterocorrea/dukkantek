using System.Net;
using Humanizer;
using Newtonsoft.Json;

namespace dukkantek.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionMiddleware(RequestDelegate next)
        => _next = next ?? throw new ArgumentNullException(nameof(next));

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = SerializeResponse(httpContext, ex, "service_error");
            await httpContext.Response.WriteAsync(response);
        }
    }
    
    private string SerializeResponse(HttpContext httpContext, Exception ex, string errorType)
    {
        var response = JsonConvert.SerializeObject(new {
                ErrorType = errorType,
                ErrorCodes = new[] { ex.GetType().Name.Underscore().ToLowerInvariant() }
            });
        return response;
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<ExceptionMiddleware>();
}