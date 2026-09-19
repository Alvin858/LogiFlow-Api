namespace LogiFlow.Domain.Entities;

public class ProofOfDelivery
{
    public int Id { get; set; }

    public int DeliveryId { get; set; }

    public string ReceiverName { get; set; } = string.Empty;

    public string SignaturePath { get; set; } = string.Empty;

    public string PhotoPath { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public DateTime CapturedAtUtc { get; set; } = DateTime.UtcNow;

    public Delivery Delivery { get; set; } = null!;
}