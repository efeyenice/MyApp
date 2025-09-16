using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

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


app.Run(async (HttpContext context) => {
    // Route based on URL segments first
    if (context.Request.Path.StartsWithSegments("/employees")) {
        await HandleEmployeesRoute(context);
    }
    else if (context.Request.Path.StartsWithSegments("/customer")) {
        await HandleCustomerRoute(context);
    }
    else if (context.Request.Path.StartsWithSegments("/product")) {
        await HandleProductRoute(context);
    }
    else if (context.Request.Path.StartsWithSegments("/order")) {
        await HandleOrderRoute(context);
    }
    else if (context.Request.Path.StartsWithSegments("/")) {
        await HandleRootRoute(context);
    }
    else if (context.Request.Path.StartsWithSegments("/httpresponse")) {
        await HandleHttpResponseRoute(context);
    }
    else {
        await context.Response.WriteAsync("Not found");
    }
});

// Route handlers for each endpoint
async Task HandleEmployeesRoute(HttpContext context)
{
    switch (context.Request.Method)
    {
        case "GET":
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/plain";
            
            // Check if an ID is provided in query parameters
            if (context.Request.Query.ContainsKey("id"))
            {
                // Display a particular employee
                var id = int.Parse(context.Request.Query["id"]);
                var employee = EmployeesRepository.GetEmployeeById(id);
                if (employee != null)
                {
                    await context.Response.WriteAsync($"Employee: {employee.Name} - {employee.Position} - {employee.Salary}\n");
                }
                else
                {
                    await context.Response.WriteAsync("Employee not found\n");
                }
            }
            else
            {
                // Display all employees
                var employees = EmployeesRepository.GetAllEmployees();
                foreach (var employee in employees)
                {
                    await context.Response.WriteAsync($"Employee: {employee.Name} - {employee.Position} - {employee.Salary}\n");
                }
            }
            break;

        //add an employee
        case "POST":
            try
            {
                using (var reader = new StreamReader(context.Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var employee = JsonSerializer.Deserialize<Employee>(body);
                    EmployeesRepository.AddEmployee(employee);
                    context.Response.StatusCode = 201;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Employee added successfully");
                }
            }
            catch (JsonException ex)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($"Invalid JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($"Error adding employee: {ex.Message}");
            }
            break;
            
        //update an employee
        case "PUT":
            try
            {
                using (var reader = new StreamReader(context.Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var employee = JsonSerializer.Deserialize<Employee>(body);
                    EmployeesRepository.UpdateEmployee(employee);
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Employee updated successfully");
                }
            }
            catch (JsonException ex)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($"Invalid JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($"Error updating employee: {ex.Message}");
            }
            break;
            
        //delete an employee
        case "DELETE":
            var deleteId = int.Parse(context.Request.Query["id"]);
            //handle auth here
            if (context.Request.Headers["Authorization"] != "admin")
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
            try
            {
                EmployeesRepository.DeleteEmployee(deleteId);
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Employee deleted successfully");
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($"Error deleting employee: {ex.Message}");
            }
            break;
            
        default:
            context.Response.StatusCode = 405; // Method Not Allowed
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("Method not allowed");
            break;
    }
}

async Task HandleCustomerRoute(HttpContext context)
{
    switch (context.Request.Method)
    {
        case "GET":
            await context.Response.WriteAsync("Customer");
            break;
        default:
            context.Response.StatusCode = 405; // Method Not Allowed
            await context.Response.WriteAsync("Method not allowed");
            break;
    }
}

async Task HandleProductRoute(HttpContext context)
{
    switch (context.Request.Method)
    {
        case "GET":
            await context.Response.WriteAsync("Product");
            break;
        default:
            context.Response.StatusCode = 405; // Method Not Allowed
            await context.Response.WriteAsync("Method not allowed");
            break;
    }
}

async Task HandleOrderRoute(HttpContext context)
{
    switch (context.Request.Method)
    {
        case "GET":
            await context.Response.WriteAsync("Order");
            break;
        default:
            context.Response.StatusCode = 405; // Method Not Allowed
            await context.Response.WriteAsync("Method not allowed");
            break;
    }
}

async Task HandleHttpResponseRoute(HttpContext context)
{
    await context.Response.WriteAsync("HttpResponse");
}

async Task HandleRootRoute(HttpContext context)
{
    if (context.Request.Method == "GET")
    {

        context.Response.Headers.Add("Content-Type", "text/html");

        //modify below to use html tags
        await context.Response.WriteAsync($"<h1>Request URL: {context.Request.Path}</h1>\n");
        await context.Response.WriteAsync($"<h1>Request Method: {context.Request.Method}</h1>\n");
        await context.Response.WriteAsync($"<h1>Headers:</h1>\n");
        foreach (var header in context.Request.Headers)
        {
            await context.Response.WriteAsync($"<h1>  {header.Key}: {header.Value}</h1>\n");
        }
        await context.Response.WriteAsync($"<h1>Request Query: {context.Request.QueryString}</h1>\n");
        await context.Response.WriteAsync($"<h1>Request Body: {context.Request.Body}</h1>\n");
        
        // Only access form data if the request has a proper content type
        if (context.Request.HasFormContentType)
        {
            try
            {
                var form = await context.Request.ReadFormAsync();
                await context.Response.WriteAsync($"<h1>Request Form: {form}</h1>\n");
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsync($"<h1>Request Form Error: {ex.Message}</h1>\n");
            }
        }
        else
        {
            await context.Response.WriteAsync("<h1>Request Form: Not available (no form content type)</h1>\n");
        }
        
        await context.Response.WriteAsync($"<h1>Request Cookies: {context.Request.Cookies}</h1>\n");
        await context.Response.WriteAsync($"<h1>Request User: {context.User?.Identity?.Name ?? "Anonymous"}</h1>\n");
        await context.Response.WriteAsync($"<h1>Request User Agent: {context.Request.Headers["User-Agent"]}</h1>\n");
    }
    else
    {
        context.Response.StatusCode = 405; // Method Not Allowed
        await context.Response.WriteAsync("Method not allowed");
    }
}


app.Run();

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public double Salary { get; set; }

    public Employee(int id, string name, string position, double salary)
    {
        Id = id;
        Name = name;
        Position = position;
        Salary = salary;
    }
}


static class EmployeesRepository
{
    private static List<Employee> Employees = new List<Employee>
    {
        new Employee(1, "John Doe", "Manager", 100000),
        new Employee(2, "Jane Smith", "Developer", 80000),
        new Employee(3, "Jim Beam", "Designer", 70000)
    };

    public static void AddEmployee(Employee employee)
    {
        Employees.Add(employee);
    }

    public static void UpdateEmployee(Employee employee)
    {
        if (employee == null)
        {
            return;
        }
        var existingEmployee = Employees.FirstOrDefault(e => e.Id == employee.Id);
        if (existingEmployee != null)
        {
            existingEmployee.Name = employee.Name;
            existingEmployee.Position = employee.Position;
            existingEmployee.Salary = employee.Salary;
        }
    }

    public static void DeleteEmployee(int id)
    {
        Employees.RemoveAll(e => e.Id == id);
    }

    public static List<Employee> GetAllEmployees()
    {
        return Employees;
    }

    public static Employee? GetEmployeeById(int id)
    {
        return Employees.FirstOrDefault(e => e.Id == id);
    }
}