using Project_01.Domain.Contracts;

namespace Project_01.Domain.Payments;

public class Payment
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentStatus Status { get; set; }

    public virtual Contract Contract { get; set; }
}