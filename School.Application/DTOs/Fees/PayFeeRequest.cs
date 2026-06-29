namespace School.Application.DTOs.Fees;

public record PayFeeRequest(Guid StudentId, decimal Amount, string? Note);