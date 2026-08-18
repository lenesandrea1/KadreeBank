using KadreeBank.Application.Exceptions;
using KadreeBank.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace KadreeBank.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (status, message) = ex switch
            {
                DomainException => (StatusCodes.Status422UnprocessableEntity, ex.Message),
                NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                ConcurrencyConflictException => (StatusCodes.Status409Conflict, ex.Message),
                ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, ex.Message),
                ArgumentException => (StatusCodes.Status400BadRequest, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Error interno.")
            };

            if (status == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(ex, "Error no controlado");
            }

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = message,
                Detail = message
            });
        }
    }
}
