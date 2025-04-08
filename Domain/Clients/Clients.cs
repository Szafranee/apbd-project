namespace Project_01.Domain.Clients;

public abstract class Client
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public ClientType ClientType { get; set; }
    public bool IsDeleted { get; set; }
}

public class IndividualClient : Client
{
    public string LastName { get; set; }
    public string PESEL { get; set; }
}

public class CorporateClient : Client
{
    public string KRS { get; set; }
}