using School.Domain.Entities;

namespace School.Application.Interfaces.Repositories;

public interface IFeePaymentRepository
{
    Task AddAsync(FeePayment payment);
    Task<List<FeePayment>> GetByStudentIdAsync(Guid studentId);
    Task<decimal> GetTotalPaidByStudentAsync(Guid studentId);
}