using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EPHRS
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Department { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public decimal MonthlySalary { get; set; }
        public double TaxRate { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; } = "";

        public decimal NetSalary => MonthlySalary - MonthlySalary * (decimal)TaxRate;
    }

    class Program
    {
        // Empty list: 0 pre-defined records
        static List<Employee> employees = new List<Employee>();
        static int nextId = 1001;

        // >>> This is the line to change differently on main and your feature branch <<<
        const string SystemTitle = "EMPLOYEE PAYROLL & HR MANAGEMENT SYSTEM";

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("\n==== " + SystemTitle + " ====");
                Console.WriteLine("1. Create Employee");
                Console.WriteLine("2. Read All Employees");
                Console.WriteLine("3. Search / Filter");
                Console.WriteLine("4. Update Employee");
                Console.WriteLine("5. Delete Employee");
                Console.WriteLine("0. Exit");
                Console.Write("Choose: ");

                switch (Console.ReadLine())
                {
                    case "1": CreateEmployee(); break;
                    case "2": ReadAll(); break;
                    case "3": Search(); break;
                    case "4": UpdateEmployee(); break;
                    case "5": DeleteEmployee(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        // ---------- CREATE ----------
        static void CreateEmployee()
        {
            var e = new Employee { Id = nextId++ };
            e.FirstName = ReadText("First Name: ");
            e.LastName = ReadText("Last Name: ");
            e.Department = ReadText("Department (IT/HR/Sales/Finance): ");
            e.JobTitle = ReadText("Job Title: ");
            e.MonthlySalary = ReadDecimal("Monthly Salary: ");
            e.TaxRate = ReadDouble("Tax Rate (e.g., 0.12): ");
            e.HireDate = ReadDate("Hire Date (yyyy-MM-dd): ");
            e.IsActive = ReadBool("Is Active? (y/n): ");
            e.Email = ReadText("Email: ");
            employees.Add(e);
            Console.WriteLine($"Employee created with Id {e.Id}.");
        }

        // ---------- READ ----------
        static void ReadAll()
        {
            Display(employees.OrderBy(e => e.Id).ToList());
        }

        // ---------- SEARCH / FILTER ----------
        static void Search()
        {
            Console.WriteLine("Search by: 1) Name  2) Department  3) Active status  4) Min salary");
            Console.Write("Choose: ");
            string? c = Console.ReadLine();
            List<Employee> result;

            switch (c)
            {
                case "1":
                    string name = ReadText("Name keyword: ").ToLower();
                    result = employees.Where(e => e.FirstName.ToLower().Contains(name)
                                               || e.LastName.ToLower().Contains(name)).ToList();
                    break;
                case "2":
                    string dept = ReadText("Department: ").ToLower();
                    result = employees.Where(e => e.Department.ToLower() == dept).ToList();
                    break;
                case "3":
                    bool active = ReadBool("Active? (y/n): ");
                    result = employees.Where(e => e.IsActive == active).ToList();
                    break;
                case "4":
                    decimal min = ReadDecimal("Minimum salary: ");
                    result = employees.Where(e => e.MonthlySalary >= min)
                                      .OrderByDescending(e => e.MonthlySalary).ToList();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }
            Display(result);
        }

        // ---------- UPDATE ----------
        static void UpdateEmployee()
        {
            int id = ReadInt("Enter Employee Id to update: ");
            var e = employees.FirstOrDefault(x => x.Id == id);
            if (e == null) { Console.WriteLine("Employee not found."); return; }

            Console.WriteLine("Leave blank to keep the current value.");
            e.FirstName = ReadOptional($"First Name [{e.FirstName}]: ", e.FirstName);
            e.LastName = ReadOptional($"Last Name [{e.LastName}]: ", e.LastName);
            e.Department = ReadOptional($"Department [{e.Department}]: ", e.Department);
            e.JobTitle = ReadOptional($"Job Title [{e.JobTitle}]: ", e.JobTitle);

            string s = ReadOptional($"Monthly Salary [{e.MonthlySalary}]: ", "");
            if (decimal.TryParse(s, out decimal sal)) e.MonthlySalary = sal;

            s = ReadOptional($"Tax Rate [{e.TaxRate}]: ", "");
            if (double.TryParse(s, out double tax)) e.TaxRate = tax;

            s = ReadOptional($"Hire Date [{e.HireDate:yyyy-MM-dd}]: ", "");
            if (DateTime.TryParse(s, out DateTime hd)) e.HireDate = hd;

            s = ReadOptional($"Is Active (y/n) [{(e.IsActive ? "y" : "n")}]: ", "");
            if (s.ToLower() == "y") e.IsActive = true;
            else if (s.ToLower() == "n") e.IsActive = false;

            e.Email = ReadOptional($"Email [{e.Email}]: ", e.Email);
            Console.WriteLine("Employee updated.");
        }

        // ---------- DELETE ----------
        static void DeleteEmployee()
        {
            int id = ReadInt("Enter Employee Id to delete: ");
            var e = employees.FirstOrDefault(x => x.Id == id);
            if (e == null) { Console.WriteLine("Employee not found."); return; }

            if (ReadBool($"Delete {e.FirstName} {e.LastName}? (y/n): "))
            {
                employees.RemoveAll(x => x.Id == id);
                Console.WriteLine("Employee deleted.");
            }
            else Console.WriteLine("Delete cancelled.");
        }

        // ---------- DISPLAY ----------
        static void Display(List<Employee> list)
        {
            if (!list.Any()) { Console.WriteLine("No records found."); return; }

            Console.WriteLine();
            Console.WriteLine($"{"Id",-6}{"First",-10}{"Last",-10}{"Dept",-9}{"Job Title",-18}{"Salary",12}{"Tax",6}{"Net",12}  {"Hired",-11}{"Active",-7}Email");
            Console.WriteLine(new string('-', 125));
            foreach (var e in list)
            {
                Console.WriteLine($"{e.Id,-6}{e.FirstName,-10}{e.LastName,-10}{e.Department,-9}{e.JobTitle,-18}" +
                                  $"{e.MonthlySalary,12:N2}{e.TaxRate,6:P0}{e.NetSalary,12:N2}  " +
                                  $"{e.HireDate:yyyy-MM-dd} {(e.IsActive ? "Yes" : "No"),-7}{e.Email}");
            }
            Console.WriteLine($"\nTotal: {list.Count} | Total Monthly Payroll: {list.Sum(x => x.MonthlySalary):N2}");
        }

        // ---------- INPUT HELPERS ----------
        static string ReadText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
                Console.WriteLine("Value cannot be empty.");
            }
        }

        static string ReadOptional(string prompt, string current)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            return string.IsNullOrWhiteSpace(s) ? current : s.Trim();
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int v)) return v;
                Console.WriteLine("Enter a valid whole number.");
            }
        }

        static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out decimal v) && v >= 0) return v;
                Console.WriteLine("Enter a valid non-negative amount.");
            }
        }

        static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out double v) && v >= 0 && v <= 1) return v;
                Console.WriteLine("Enter a rate between 0 and 1 (e.g., 0.12).");
            }
        }

        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime v)) return v;
                Console.WriteLine("Use the format yyyy-MM-dd.");
            }
        }

        static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? s = Console.ReadLine()?.Trim().ToLower();
                if (s == "y" || s == "yes") return true;
                if (s == "n" || s == "no") return false;
                Console.WriteLine("Enter y or n.");
            }
        }
    }
}
