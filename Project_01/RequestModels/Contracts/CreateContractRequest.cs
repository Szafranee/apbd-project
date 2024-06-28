namespace Project_01.RequestModels.Contracts;

public class CreateContractRequest
{
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsSigned { get; set; }
    public int SupportYears { get; set; }
}