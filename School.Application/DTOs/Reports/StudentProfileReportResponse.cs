using School.Application.DTOs.Fees;

namespace School.Application.DTOs.Reports;

public record StudentProfileReportResponse(
    Guid StudentId,
    string StudentName,
    string Class,
    int Age,
    decimal TotalFee,
    decimal PaidAmount,
    decimal DueAmount,
    int PresentCount,
    int AbsentCount,
    int NotMarkedCount
);