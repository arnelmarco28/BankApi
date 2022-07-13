using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Bank.WebApi
{
    /// <summary>
    /// global error handling 
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {                
                _logger.LogError($"Message:{ex.Message}  StackTrace:{ex.StackTrace}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log issues and handle exception response
            if (exception is ValidationException validationException)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var errors = validationException
                    .Errors
                    .Select(w => new { w.PropertyName, w.ErrorMessage });
                var result = JsonConvert.SerializeObject(errors);

                return context.Response.WriteAsync(result);
            }
            else
            {
                var result = JsonConvert.SerializeObject(new { isSuccess = false, error = exception.Message });

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                return context.Response.WriteAsync(result);
            }
        }
    }

    public static class ErrorHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}
