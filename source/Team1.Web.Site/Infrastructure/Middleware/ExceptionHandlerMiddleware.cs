using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Team1.Entities;
using Team1.Infrastructure.Services.Logs;
using Team1.Infrastructure.UserIdentity;
using Team1.Web.Site.Infrastructure.Middleware;
using System;
using System.Threading.Tasks;

namespace Team1.Web.Site.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware that handles uncaught exceptions
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, SystemLogsCreate systemLogsCreate, DataContext spudContext, ILoggingContext loggingDb, UserPermissionService userPermissionService)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                systemLogsCreate.SetupService(spudContext, loggingDb, userPermissionService);
                await HandleExceptionAsync(context, ex, systemLogsCreate);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, SystemLogsCreate systemLogsCreate)
        {
            await systemLogsCreate.SaveLog(Model.Enums.LogTypeEnum.UncaughtError, exception);

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            await response.WriteAsync(JsonConvert.SerializeObject(new
            {
                // customize as you need
                error = new
                {
                    message = exception.Message,
                    exception = exception.GetType().Name
                }
            }));
        }
    }
}

/// <summary>
/// used to turn on the middleware
/// </summary>
public static class ErrorHandlingMiddlewareExtensions
{
    /// <summary>
    /// Turns on the error handling middleware
    /// </summary>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
