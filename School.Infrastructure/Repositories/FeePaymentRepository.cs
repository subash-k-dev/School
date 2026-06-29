using Microsoft.EntityFrameworkCore;
using School.Application.Interfaces.Repositories;
using School.Domain.Entities;
using School.Infrastructure.Persistence;

namespace School.Infrastructure.Repositories;

public class FeePaymentRepository : IFeePaymentRepository
{
    private readonly AppDbContext _db;

    public FeePaymentRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(FeePayment payment)
    {
        _db.FeePayments.Add(payment);
        await _db.SaveChangesAsync();
    }

    public Task<List<FeePayment>> GetByStudentIdAsync(Guid studentId)
        => _db.FeePayments.AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .ToListAsync();

    public async Task<decimal> GetTotalPaidByStudentAsync(Guid studentId)
    {
        return await _db.FeePayments
            .Where(x => x.StudentId == studentId)
            .SumAsync(x => (decimal?)x.Amount) ?? 0;
    }
}