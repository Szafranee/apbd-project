using System.ComponentModel.DataAnnotations;

namespace Project_01.RequestModels.Clients;

public class UpdateClientRequest
{
    [Required]
    public string Name { get; set; }
    public string? LastName { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }

    [Required]
    public string ClientType { get; set; }
    public string? PESEL { get; set; }
    public string? KRS { get; set; }
}