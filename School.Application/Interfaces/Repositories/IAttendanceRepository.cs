using School.Domain.Entities;

namespace School.Application.Interfaces.Repositories;

public interface IAttendanceRepository
{
    Task<AttendanceRecord?> GetByStudentAndDateAsync(Guid studentId, DateTime date);
    Task AddAsync(AttendanceRecord record);
    Task UpdateAsync(AttendanceRecord record);
    Task<List<AttendanceRecord>> GetByStudentAsync(Guid studentId);
    Task<List<AttendanceRecord>> GetByDateAsync(DateTime date);
}