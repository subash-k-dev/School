namespace School.Application.DTOs.Reports;

public record FeeDueReportItem(
    Guid StudentId,
    string StudentName,
    string Class,
    decimal TotalFee,
    decimal PaidAmount,
    decimal DueAmount
);