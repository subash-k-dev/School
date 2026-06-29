using Microsoft.EntityFrameworkCore;
using School.Application.Interfaces.Repositories;
using School.Domain.Entities;
using School.Infrastructure.Persistence;

namespace School.Infrastructure.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext _db;

    public AttendanceRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<AttendanceRecord?> GetByStudentAndDateAsync(Guid studentId, DateTime date)
        => _db.AttendanceRecords.FirstOrDefaultAsync(x => x.StudentId == studentId && x.Date == date.Date);

    public async Task AddAsync(AttendanceRecord record)
    {
        _db.AttendanceRecords.Add(record);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(AttendanceRecord record)
    {
        _db.AttendanceRecords.Update(record);
        await _db.SaveChangesAsync();
    }

    public Task<List<AttendanceRecord>> GetByStudentAsync(Guid studentId)
        => _db.AttendanceRecords.AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.Date)
            .ToListAsync();

    public Task<List<AttendanceRecord>> GetByDateAsync(DateTime date)
        => _db.AttendanceRecords.AsNoTracking()
            .Where(x => x.Date == date.Date)
            .ToListAsync();
}