namespace LogiFlow.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int ShipmentId { get; set; }
    public int CustomerId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Issued";
    public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DueDateUtc { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public Customer Customer { get; set; } = null!;
    public Shipment Shipment { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
