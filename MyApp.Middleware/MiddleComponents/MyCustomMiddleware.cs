//create a custom middleware
public class MyCustomMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Add your middleware logic here
        Console.WriteLine("🔧 MyCustomMiddleware: BEFORE calling next middleware");
        Console.WriteLine($"   Request Path: {context.Request.Path}");

        // Call the next middleware in the pipeline
        await next(context);

        Console.WriteLine("🔧 MyCustomMiddleware: AFTER calling next middleware");
        Console.WriteLine($"   Response Status: {context.Response.StatusCode}");
    }
}