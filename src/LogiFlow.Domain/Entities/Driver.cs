namespace LogiFlow.Domain.Entities;

public class Driver
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime LicenseExpiryDate { get; set; }
    public int ExperienceYears { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public User User { get; set; } = null!;
}
