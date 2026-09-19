namespace LogiFlow.Domain.Entities;

public class CustomerAddress
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = "India";
    public bool IsDefault { get; set; }

    public Customer Customer { get; set; } = null!;
}
