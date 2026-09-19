using LogiFlow.Domain.Enums;

namespace LogiFlow.Domain.Entities;

public class Schedule
{
    public int Id { get; set; }

    public int ShipmentId { get; set; }

    public int DriverId { get; set; }

    public int VehicleId { get; set; }

    public DateTime StartTimeUtc { get; set; }

    public DateTime EndTimeUtc { get; set; }

    public ScheduleStatus Status { get; set; } = ScheduleStatus.Scheduled;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }
}