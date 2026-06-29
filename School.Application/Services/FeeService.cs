using School.Application.DTOs.Fees;
using School.Application.Interfaces.Repositories;
using School.Domain.Entities;

namespace School.Application.Services;

public class FeeService
{
    private readonly IFeePaymentRepository _feeRepo;
    private readonly IStudentRepository _studentRepo;

    public FeeService(IFeePaymentRepository feeRepo, IStudentRepository studentRepo)
    {
        _feeRepo = feeRepo;
        _studentRepo = studentRepo;
    }

    public async Task<FeePaymentResponse?> PayAsync(PayFeeRequest req)
    {
        var student = await _studentRepo.GetByIdAsync(req.StudentId);
        if (student is null) return null;

        if (req.Amount <= 0) throw new ArgumentException("Amount must be > 0");

        var payment = new FeePayment
        {
            StudentId = req.StudentId,
            Amount = req.Amount,
            PaidOn = DateTime.UtcNow,
            Note = req.Note
        };

        await _feeRepo.AddAsync(payment);

        return new FeePaymentResponse(payment.Id, payment.StudentId, payment.Amount, payment.PaidOn, payment.Note);
    }

    public async Task<List<FeePaymentResponse>?> GetStudentPaymentsAsync(Guid studentId)
    {
        var student = await _studentRepo.GetByIdAsync(studentId);
        if (student is null) return null;

        var payments = await _feeRepo.GetByStudentIdAsync(studentId);

        return payments
            .OrderByDescending(p => p.PaidOn)
            .Select(p => new FeePaymentResponse(p.Id, p.StudentId, p.Amount, p.PaidOn, p.Note))
            .ToList();
    }

    public async Task<StudentFeeSummaryResponse?> GetStudentSummaryAsync(Guid studentId)
    {
        var student = await _studentRepo.GetByIdAsync(studentId);
        if (student is null) return null;

        var paid = await _feeRepo.GetTotalPaidByStudentAsync(studentId);
        var due = Math.Max(0, student.TotalFee - paid);

        return new StudentFeeSummaryResponse(student.Id, student.Name, student.Class, student.TotalFee, paid, due);
    }

    public async Task<List<StudentFeeSummaryResponse>> GetDuesAsync()
    {
        var students = await _studentRepo.GetAllAsync();

        var list = new List<StudentFeeSummaryResponse>();

        foreach (var s in students)
        {
            var paid = await _feeRepo.GetTotalPaidByStudentAsync(s.Id);
            var due = Math.Max(0, s.TotalFee - paid);

            if (due > 0)
                list.Add(new StudentFeeSummaryResponse(s.Id, s.Name, s.Class, s.TotalFee, paid, due));
        }

        return list.OrderByDescending(x => x.DueAmount).ToList();
    }
}