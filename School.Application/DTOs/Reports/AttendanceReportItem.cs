namespace School.Application.DTOs.Reports;

public record AttendanceReportItem(
    Guid StudentId,
    string StudentName,
    string Class,
    DateTime Date,
    string Status,
    string MarkedBy
);