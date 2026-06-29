namespace School.Application.DTOs.Fees;

public record StudentFeeSummaryResponse(
    Guid StudentId,
    string StudentName,
    string Class,
    decimal TotalFee,
    decimal PaidAmount,
    decimal DueAmount
);