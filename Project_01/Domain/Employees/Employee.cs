namespace Project_01.Domain.Employees;

public class Employee
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    public string Role { get; set; } // "Admin" or "Standard"
}