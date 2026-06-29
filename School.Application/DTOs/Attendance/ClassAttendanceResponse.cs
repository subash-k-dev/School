namespace School.Application.DTOs.Attendance;

public record ClassAttendanceResponse(
    Guid StudentId,
    string StudentName,
    string Class,
    DateTime Date,
    string Status,
    string MarkedBy
);