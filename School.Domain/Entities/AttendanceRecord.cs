using School.Domain.Entities.Enums;

namespace School.Domain.Entities;

public class AttendanceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid StudentId { get; set; }

    // Store only date part (we will always send yyyy-MM-dd)
    public DateTime Date { get; set; }

    public AttendanceStatus Status { get; set; }

    public string MarkedBy { get; set; } = string.Empty;
}