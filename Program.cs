using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
 
List<Employee> employees = new();   // starts with 0 records
int nextId = 1001;
 
bool running = true;
while (running)
{
    Console.WriteLine();
    Console.WriteLine("=== EPHRS - Feature Branch ===");
    Console.WriteLine("1. Add Employee");
    Console.WriteLine("2. View All Employees");
    Console.WriteLine("3. Search / Filter");
    Console.WriteLine("4. Update Employee");
    Console.WriteLine("5. Delete Employee");
    Console.WriteLine("0. Exit");
    Console.Write("Choose an option: ");
 
    switch (Console.ReadLine())
    {
        case "1": AddEmployee(); break;
        case "2": ViewAll(); break;
        case "3": SearchEmployees(); break;
        case "4": UpdateEmployee(); break;
        case "5": DeleteEmployee(); break;
        case "0": running = false; break;
        default: Console.WriteLine("Invalid choice. Try again."); break;
    }
}
 
// ---------------- CREATE ----------------
void AddEmployee()
{
    Console.WriteLine("\n--- Add Employee ---");
    var emp = new Employee
    {
        Id = nextId++,
        FirstName = ReadString("First name: "),
        LastName = ReadString("Last name: "),
        Department = ReadString("Department (IT, HR, Sales, Finance): "),
        JobTitle = ReadString("Job title: "),
        MonthlySalary = ReadDecimal("Monthly salary: "),
        TaxRate = ReadDouble("Tax rate (e.g. 0.12 for 12%): "),
        HireDate = ReadDate("Hire date (yyyy-MM-dd): "),
        IsActive = ReadBool("Is active? (y/n): "),
        Email = ReadString("Email: ")
    };
    employees.Add(emp);
    Console.WriteLine($"Employee added with Id {emp.Id}.");
}
 
// ---------------- READ ----------------
void ViewAll()
{
    Console.WriteLine("\n--- All Employees ---");
    PrintEmployees(employees.OrderBy(e => e.Id));
}
 
// ---------------- SEARCH / FILTER ----------------
void SearchEmployees()
{
    Console.WriteLine("\n--- Search / Filter ---");
    Console.WriteLine("1. By name");
    Console.WriteLine("2. By department");
    Console.WriteLine("3. By status (active / inactive)");
    Console.WriteLine("4. By Id");
    Console.Write("Choose: ");
 
    switch (Console.ReadLine())
    {
        case "1":
            string name = ReadString("Enter name to search: ");
            PrintEmployees(employees.Where(e =>
                e.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                e.LastName.Contains(name, StringComparison.OrdinalIgnoreCase)));
            break;
        case "2":
            string dept = ReadString("Enter department: ");
            PrintEmployees(employees.Where(e =>
                e.Department.Contains(dept, StringComparison.OrdinalIgnoreCase)));
            break;
        case "3":
            bool active = ReadBool("Show active employees? (y = active, n = inactive): ");
            PrintEmployees(employees.Where(e => e.IsActive == active));
            break;
        case "4":
            int id = ReadInt("Enter Id: ");
            PrintEmployees(employees.Where(e => e.Id == id));
            break;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }
}
 
// ---------------- UPDATE ----------------
void UpdateEmployee()
{
    Console.WriteLine("\n--- Update Employee ---");
    if (!employees.Any())
    {
        Console.WriteLine("No records to update.");
        return;
    }
 
    int id = ReadInt("Enter Id of employee to update: ");
    var emp = employees.FirstOrDefault(e => e.Id == id);
    if (emp == null)
    {
        Console.WriteLine("Employee not found.");
        return;
    }
 
    Console.WriteLine("Press Enter to keep the current value.");
 
    string input;
 
    Console.Write($"First name [{emp.FirstName}]: ");
    input = Console.ReadLine() ?? "";
    if (input.Trim() != "") emp.FirstName = input.Trim();
 
    Console.Write($"Last name [{emp.LastName}]: ");
    input = Console.ReadLine() ?? "";
    if (input.Trim() != "") emp.LastName = input.Trim();
 
    Console.Write($"Department [{emp.Department}]: ");
    input = Console.ReadLine() ?? "";
    if (input.Trim() != "") emp.Department = input.Trim();
 
    Console.Write($"Job title [{emp.JobTitle}]: ");
    input = Console.ReadLine() ?? "";
    if (input.Trim() != "") emp.JobTitle = input.Trim();
 
    Console.Write($"Monthly salary [{emp.MonthlySalary}]: ");
    input = Console.ReadLine() ?? "";
    if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal salary) && salary >= 0)
        emp.MonthlySalary = salary;
 
    Console.Write($"Tax rate [{emp.TaxRate}]: ");
    input = Console.ReadLine() ?? "";
    if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double tax) && tax >= 0 && tax <= 1)
        emp.TaxRate = tax;
 
    Console.Write($"Hire date [{emp.HireDate:yyyy-MM-dd}]: ");
    input = Console.ReadLine() ?? "";
    if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime hired))
        emp.HireDate = hired;
 
    Console.Write($"Is active? (y/n) [{(emp.IsActive ? "y" : "n")}]: ");
    input = (Console.ReadLine() ?? "").Trim().ToLower();
    if (input == "y") emp.IsActive = true;
    else if (input == "n") emp.IsActive = false;
 
    Console.Write($"Email [{emp.Email}]: ");
    input = Console.ReadLine() ?? "";
    if (input.Trim() != "") emp.Email = input.Trim();
 
    Console.WriteLine("Employee updated.");
    PrintEmployees(employees.Where(e => e.Id == id));
}
 
// ---------------- DELETE ----------------
void DeleteEmployee()
{
    Console.WriteLine("\n--- Delete Employee ---");
    if (!employees.Any())
    {
        Console.WriteLine("No records to delete.");
        return;
    }
 
    int id = ReadInt("Enter Id of employee to delete: ");
    var emp = employees.FirstOrDefault(e => e.Id == id);
    if (emp == null)
    {
        Console.WriteLine("Employee not found.");
        return;
    }
 
    if (ReadBool($"Delete {emp.FirstName} {emp.LastName}? (y/n): "))
    {
        employees.RemoveAll(e => e.Id == id);
        Console.WriteLine("Employee deleted.");
    }
    else
    {
        Console.WriteLine("Delete cancelled.");
    }
}
 
// ---------------- HELPERS ----------------
void PrintEmployees(IEnumerable<Employee> list)
{
    var items = list.ToList();
    if (items.Count == 0)
    {
        Console.WriteLine("No records found.");
        return;
    }
 
    Console.WriteLine(
        $"{"Id",-6}{"First",-12}{"Last",-12}{"Dept",-10}{"Job Title",-18}" +
        $"{"Salary",12}{"Tax",7}{"Net Salary",13}  {"Hired",-11}{"Active",-8}Email");
    Console.WriteLine(new string('-', 140));
 
    foreach (var e in items)
    {
        decimal net = e.MonthlySalary - (e.MonthlySalary * (decimal)e.TaxRate);
        Console.WriteLine(
            $"{e.Id,-6}{e.FirstName,-12}{e.LastName,-12}{e.Department,-10}{e.JobTitle,-18}" +
            $"{e.MonthlySalary,12:N2}{e.TaxRate,7:P0}{net,13:N2}  {e.HireDate,-11:yyyy-MM-dd}{e.IsActive,-8}{e.Email}");
    }
    Console.WriteLine($"Total records: {items.Count}");
}
 
string ReadString(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string input = (Console.ReadLine() ?? "").Trim();
        if (input != "") return input;
        Console.WriteLine("This field cannot be empty.");
    }
}
 
int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int value)) return value;
        Console.WriteLine("Please enter a valid whole number.");
    }
}
 
decimal ReadDecimal(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value) && value >= 0)
            return value;
        Console.WriteLine("Please enter a valid amount (0 or more).");
    }
}
 
double ReadDouble(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
            && value >= 0 && value <= 1)
            return value;
        Console.WriteLine("Please enter a rate between 0 and 1 (e.g. 0.12).");
    }
}
 
DateTime ReadDate(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (DateTime.TryParse(Console.ReadLine(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value))
            return value;
        Console.WriteLine("Please enter a valid date (yyyy-MM-dd).");
    }
}
 
bool ReadBool(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string input = (Console.ReadLine() ?? "").Trim().ToLower();
        if (input == "y" || input == "yes") return true;
        if (input == "n" || input == "no") return false;
        Console.WriteLine("Please enter y or n.");
    }
}
 
// ---------------- MODEL ----------------
public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Department { get; set; } = "";
    public string JobTitle { get; set; } = "";
    public decimal MonthlySalary { get; set; }
    public double TaxRate { get; set; }          // 0.12 = 12%
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    public string Email { get; set; } = "";
}