var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.MapGet("/", () => "Welcome to the Employee Management API!");





//model binding
app.MapGet("/employees/{id:int}", ([AsParameters] GetEmployeeParameter param) => 
{
    var employee = EmployeesRepository.GetEmployeeById(param.Id);
    
    if (employee is null)
    {
        return Results.NotFound();
    }

    if (!string.IsNullOrEmpty(param.Name))
    {
        employee.Name = param.Name;
    }

    if (!string.IsNullOrEmpty(param.Position))
    {
        employee.Position = param.Position;
    }
    return Results.Ok(employee);
});


//bind arrays to query string or headers
app.MapGet("/employees/filter", (HttpRequest request) => 
{
    var idsFromQuery = request.Query["ids"].ToString().Split(',').Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null).Where(id => id.HasValue).Select(id => id.Value).ToList();
    var idsFromHeader = request.Headers["X-Employee-Ids"].ToString().Split(',').Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null).Where(id => id.HasValue).Select(id => id.Value).ToList();

    var filteredEmployees = EmployeesRepository.GetAllEmployees().Where(e => idsFromQuery.Contains(e.Id) || idsFromHeader.Contains(e.Id)).ToList();

    return Results.Ok(filteredEmployees);
});









//create the routes for CRUD operations
app.MapGet("/employees", () => EmployeesRepository.GetAllEmployees());
app.MapGet("/employees/{id}", (int id) => EmployeesRepository.GetEmployeeById(id) is Employee employee ? Results.Ok(employee) : Results.NotFound());
app.MapPost("/employees", (Employee employee) => { EmployeesRepository.AddEmployee(employee); return Results.Created($"/employees/{employee.Id}", employee); });
app.MapPut("/employees/{id}", (int id, Employee updatedEmployee) => 
{
    var existingEmployee = EmployeesRepository.GetEmployeeById(id);
    if (existingEmployee is null) return Results.NotFound();
    updatedEmployee.Id = id;
    EmployeesRepository.UpdateEmployee(updatedEmployee);
    return Results.NoContent();
});
app.MapDelete("/employees/{id}", (int id) => 
{
    var existingEmployee = EmployeesRepository.GetEmployeeById(id);
    if (existingEmployee is null) return Results.NotFound();
    EmployeesRepository.DeleteEmployee(id);
    return Results.NoContent();
});
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

class GetEmployeeParameter
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Position { get; set; }
}