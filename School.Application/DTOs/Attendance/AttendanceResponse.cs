namespace School.Application.DTOs.Attendance;

public record AttendanceResponse(Guid Id, Guid StudentId, DateTime Date, string Status, string MarkedBy);