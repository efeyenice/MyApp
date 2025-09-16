//create a exception middleware
public class MyExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            Console.WriteLine("🔍 Exception Middleware: BEFORE calling next middleware");
            Console.WriteLine($"   Request Path: {context.Request.Path}");
            // Set breakpoint here to debug!

            await next(context);

            Console.WriteLine("🔍 Exception Middleware: AFTER calling next middleware");
            Console.WriteLine($"   Response Status: {context.Response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 Exception Middleware: {ex.Message}");
            Console.WriteLine($"   Request Path: {context.Request.Path}");
            // Set breakpoint here to debug!

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal server error");
        }
    }
}