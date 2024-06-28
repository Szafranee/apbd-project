using Project_01.Domain.Employees;

namespace Project_01.Domain.RefreshTokens;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
}