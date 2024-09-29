using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApplication1.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class MyCustomConventionalMiddleware
    {
        private readonly RequestDelegate _next;

        public MyCustomConventionalMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            //before logic example
            if (httpContext.Request.Query.ContainsKey("firstName") && httpContext.Request.Query.ContainsKey("lastName"))
            {
                string fullName = "Hello from "+httpContext.Request.Query["firstName"]
                    + " " + httpContext.Request.Query["lastName"];
                await httpContext.Response.WriteAsync(fullName+"\n");
            }
            await _next(httpContext);
            //after logic can be put here
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMyCustomConventionalMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyCustomConventionalMiddleware>();
        }
    }
}
