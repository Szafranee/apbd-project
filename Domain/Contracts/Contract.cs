using Project_01.Domain.Clients;
using Project_01.Domain.Payments;
using Project_01.Domain.Products;

namespace Project_01.Domain.Contracts;

public class Contract
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsSigned { get; set; }
    public int SupportYears { get; set; }
    public ContractStatus Status { get; set; }

    public virtual Client Client { get; set; }
    public virtual Product Product { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }
}