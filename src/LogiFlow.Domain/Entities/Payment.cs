namespace LogiFlow.Domain.Entities;

public class Payment
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string PaymentReference { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAtUtc { get; set; }
    public Invoice Invoice { get; set; } = null!;
}
