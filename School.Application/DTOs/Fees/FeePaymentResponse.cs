namespace School.Application.DTOs.Fees;

public record FeePaymentResponse(Guid Id, Guid StudentId, decimal Amount, DateTime PaidOn, string? Note);