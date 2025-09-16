var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("position", typeof(PositionConsrtraint));
});

app.UseRouting();

//old way of doing routing
/*
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", () => "Hello World");
    endpoints.MapGet("/employees", async (HttpContext context) =>
    {
        context.Response.StatusCode = 200;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Get Employees");
    });

    endpoints.MapPost("/employees", async (HttpContext context) =>
    {
        context.Response.StatusCode = 200;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Post Employees");
    });
});
*/




//new way of doing routing
app.MapGet("/", () => "Hello World");


app.MapGet("/employees", async (HttpContext context) =>
{
    context.Response.StatusCode = 200;
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync("Get Employees");
});

app.MapGet("/employees/{id}", async (HttpContext context) =>
{
    var id = context.Request.RouteValues["id"];
    context.Response.StatusCode = 200;
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync($"Get Employees with id: {id}");
});
 

app.MapPost("/employees", async (HttpContext context) =>
{
    context.Response.StatusCode = 200;
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync("Post Employees");
});

app.MapPut("/employees", async (HttpContext context) =>
{
    context.Response.StatusCode = 200;
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync("Put Employees");
});

app.MapDelete("/employees/{id}", async (HttpContext context) =>
{
    context.Response.StatusCode = 200;
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync("Delete Employees");
});

//one required parameter with default value and one optional parameter
app.MapGet("/products/{size=medium}/{colour?}", async (HttpContext context) =>
{
    var size = context.Request.RouteValues["size"];
    var color = context.Request.RouteValues["color"] as string ?? "defaultColor";  //avoid null reference
    context.Response.StatusCode = 200;
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync($"Get Products with size: {size} and color: {color}");
});


//custom parameter constraints
app.MapGet("/orders/{id:int}", async (HttpContext context) =>
{
    var id = context.Request.RouteValues["id"];
    context.Response.StatusCode = 200;
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync($"Get Orders with id: {id}");
});



//use of custom route constraint
app.MapGet("/employees/{position:position}", async (HttpContext context) =>
{
    var position = context.Request.RouteValues["position"];
    context.Response.StatusCode = 200;
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync($"Get Staff with position: {position}");
});


app.Run();

class PositionConsrtraint : IRouteConstraint
{
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (values.TryGetValue(routeKey, out var value) && value != null)
        {
            var stringValue = Convert.ToString(value);
            if (stringValue == "admin" || stringValue == "manager" || stringValue == "developer")
            {
                return true;
            }
        }
        return false;
    }
}
