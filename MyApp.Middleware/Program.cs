var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


//there are different ways to add middleware to the container
//1. AddTransient
//2. AddScoped
//3. AddSingleton
//4. AddTransient<IMiddleware, MyCustomMiddleware>()
//5. AddScoped<IMiddleware, MyCustomMiddleware>()
//6. AddSingleton<IMiddleware, MyCustomMiddleware>()
builder.Services.AddTransient<MyCustomMiddleware>();
builder.Services.AddTransient<MyExceptionMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();










//create a exception middleware
app.UseMiddleware<MyExceptionMiddleware>();


//Middleware 4
app.UseMiddleware<MyCustomMiddleware>();


/* old way of doing branching
app.MapWhen(context =>
{
    return context.Request.Query.ContainsKey("id") && context.Request.Query["id"] == "1" && context.Request.Path.StartsWithSegments("/employees");
}, (appBuilder) =>
{
    appBuilder.Use(async (HttpContext context, RequestDelegate next) =>
    {
        Console.WriteLine("🔍 Middleware 5: BEFORE calling next middleware");
        // Set breakpoint here to debug!

        await next(context);

        Console.WriteLine("🔍 Middleware 5: AFTER calling next middleware");
        Console.WriteLine($"   Response Status: {context.Response.StatusCode}");
    });

    appBuilder.Use(async (HttpContext context, RequestDelegate next) =>
    {
        Console.WriteLine("🔍 Middleware 6: BEFORE calling next middleware");
        // Set breakpoint here to debug!

        await next(context);

        Console.WriteLine("🔍 Middleware 6: AFTER calling next middleware");
        Console.WriteLine($"   Response Status: {context.Response.StatusCode}");
    });
});
*/


app.Use(async (HttpContext context, RequestDelegate next) =>
{
    Console.WriteLine("🌍 Middleware 1: BEFORE calling next middleware");
    // Set breakpoint here to debug!

    await next(context);

    Console.WriteLine("🌍 Middleware 1: AFTER calling next middleware");
    Console.WriteLine($"   Response Status: {context.Response.StatusCode}");
});


//different kind of branching
app.UseWhen(context =>{
    return context.Request.Query.ContainsKey("id") && context.Request.Query["id"] == "1" && context.Request.Path.StartsWithSegments("/employees");
},(appBuilder) =>
{
    appBuilder.Use(async (HttpContext context, RequestDelegate next) =>
    {
        Console.WriteLine("🔍 Middleware 5: BEFORE calling next middleware");
        // Set breakpoint here to debug!

        await next(context);

        Console.WriteLine("🔍 Middleware 5: AFTER calling next middleware");
        Console.WriteLine($"   Response Status: {context.Response.StatusCode}");
    });

    appBuilder.Use(async (HttpContext context, RequestDelegate next) =>
    {
        Console.WriteLine("🔍 Middleware 6: BEFORE calling next middleware");
        // Set breakpoint here to debug!

        await next(context);

        Console.WriteLine("🔍 Middleware 6: AFTER calling next middleware");
        Console.WriteLine($"   Response Status: {context.Response.StatusCode}");
    });
});


app.Map("/employees", (appBuilder) =>
{
    appBuilder.Use(async (HttpContext context, RequestDelegate next) =>
    {
        Console.WriteLine("🔍 Middleware 5: BEFORE calling next middleware");
        Console.WriteLine($"   Request Path: {context.Request.Path}");
        // Set breakpoint here to debug!

        await next(context);

        Console.WriteLine("🔍 Middleware 5: AFTER calling next middleware");
        Console.WriteLine($"   Response Status: {context.Response.StatusCode}");
    });

    appBuilder.Use(async (HttpContext context, RequestDelegate next) =>
    {
        Console.WriteLine("🔍 Middleware 6: BEFORE calling next middleware");
        Console.WriteLine($"   Request Path: {context.Request.Path}");
        // Set breakpoint here to debug!

        await next(context);

        Console.WriteLine("🔍 Middleware 6: AFTER calling next middleware");
        Console.WriteLine($"   Response Status: {context.Response.StatusCode}");
    });

});





//Middleware 2 - DEBUG VERSION
app.Use(async (HttpContext context, RequestDelegate next) => {
    Console.WriteLine("⏱️  Middleware 2: BEFORE calling next middleware");
    // Set breakpoint here to debug timing!
    
    await next(context);
    
    Console.WriteLine("⏱️  Middleware 2: AFTER calling next middleware");
});

//Middleware 3 - DEBUG VERSION  
app.Use(async (HttpContext context, RequestDelegate next) => {
    Console.WriteLine("📋 Middleware 3: BEFORE calling next middleware");
    // Set breakpoint here to debug header addition!
    
    await next(context);
    
    Console.WriteLine("📋 Middleware 3: AFTER calling next middleware");
});



app.Run();
