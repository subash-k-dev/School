namespace School.Domain.Entities;

public class FeePayment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid StudentId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaidOn { get; set; } = DateTime.UtcNow;

    public string? Note { get; set; }
}