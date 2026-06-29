namespace School.Application.DTOs.Attendance;

// Date should be sent as "yyyy-MM-dd"
public record MarkAttendanceRequest(Guid StudentId, DateTime Date, string Status);