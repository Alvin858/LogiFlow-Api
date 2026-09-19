namespace LogiFlow.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string? TaxNumber { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
}
