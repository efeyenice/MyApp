using System.ComponentModel.DataAnnotations;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");



app.MapPost("/employees", (Employee employee) =>
{
    var validationContext = new ValidationContext(employee);
    var validationResults = new List<ValidationResult>();

    if (!Validator.TryValidateObject(employee, validationContext, validationResults, true))
    {
        return Results.BadRequest(validationResults);
    }

    // In a real application, you would save the employee to a database here.

    return Results.Created($"/employees/{employee.Id}", employee);
});


app.Run();





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


public class Employee
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
    public string Position { get; set; }
    
    [Required]
    [Range(0, 200000 ErrorMessage = "Salary must be [0, 200,000]")]
    public decimal Salary { get; set; }


    public Employee(int id, string name, string position, decimal salary)
    {
        Id = id;
        Name = name;
        Position = position;
        Salary = salary;
    }
}
