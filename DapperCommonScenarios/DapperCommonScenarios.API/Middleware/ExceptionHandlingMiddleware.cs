using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DapperCommonScenarios.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var projectName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

                Log.Fatal(ex, "{ProjectName} - Exception: {ErrorMessage}", projectName, ex.Message);

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync($"{projectName} - Exception: {ex.Message}");
                }
                else
                {
                    Log.Warning("{ProjectName} - The response has already started, the http status code middleware will not be executed.", projectName);
                    throw;
                }
            }
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
