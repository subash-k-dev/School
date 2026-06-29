namespace School.Application.DTOs.Reports;

public record AttendanceReportResponse(
    string Class,
    DateTime FromDate,
    DateTime ToDate,
    List<AttendanceReportItem> Items
);