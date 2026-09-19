using System.Text.Json;
using FluentValidation;
using LogiFlow.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteErrorAsync(context, ex);
        }
    }
    private static async Task WriteErrorAsync(HttpContext context, Exception ex)
    {
        var (status, title) = ex switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            BadRequestException => (StatusCodes.Status400BadRequest, "Bad Request"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ValidationException => (StatusCodes.Status400BadRequest, "Validation Failed"),
            DbUpdateException => (StatusCodes.Status409Conflict, "Database Update Failed"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        var detail = ex is ValidationException ve ? string.Join("; ", ve.Errors.Select(e => e.ErrorMessage)) : status == 500 ? "An unexpected error occurred." : ex.Message;
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { status, title, detail, traceId = context.TraceIdentifier }));
    }
}
