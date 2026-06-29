using School.Application.DTOs.Attendance;
using School.Application.Interfaces.Repositories;
using School.Domain.Entities;
using School.Domain.Entities.Enums;

namespace School.Application.Services;

public class AttendanceService
{
    private readonly IAttendanceRepository _repo;
    private readonly IStudentRepository _students;

    public AttendanceService(IAttendanceRepository repo, IStudentRepository students)
    {
        _repo = repo;
        _students = students;
    }

    private static AttendanceStatus ParseStatus(string status)
    {
        return status.Trim().ToLower() switch
        {
            "present" => AttendanceStatus.Present,
            "absent" => AttendanceStatus.Absent,
            _ => throw new ArgumentException("Status must be Present or Absent")
        };
    }

    private static DateTime DateOnly(DateTime dt) => dt.Date;

    public async Task<AttendanceResponse?> MarkAsync(MarkAttendanceRequest req, string markedBy)
    {
        var student = await _students.GetByIdAsync(req.StudentId);
        if (student is null) return null;

        var date = DateOnly(req.Date);
        var status = ParseStatus(req.Status);

        var existing = await _repo.GetByStudentAndDateAsync(req.StudentId, date);

        if (existing is null)
        {
            var record = new AttendanceRecord
            {
                StudentId = req.StudentId,
                Date = date,
                Status = status,
                MarkedBy = markedBy
            };

            await _repo.AddAsync(record);

            return new AttendanceResponse(record.Id, record.StudentId, record.Date, record.Status.ToString(), record.MarkedBy);
        }
        else
        {
            existing.Status = status;
            existing.MarkedBy = markedBy;

            await _repo.UpdateAsync(existing);

            return new AttendanceResponse(existing.Id, existing.StudentId, existing.Date, existing.Status.ToString(), existing.MarkedBy);
        }
    }

    public async Task<List<AttendanceResponse>?> GetStudentHistoryAsync(Guid studentId)
    {
        var student = await _students.GetByIdAsync(studentId);
        if (student is null) return null;

        var list = await _repo.GetByStudentAsync(studentId);

        return list.OrderByDescending(x => x.Date)
            .Select(x => new AttendanceResponse(x.Id, x.StudentId, x.Date, x.Status.ToString(), x.MarkedBy))
            .ToList();
    }

    public async Task<List<ClassAttendanceResponse>> GetClassAttendanceAsync(string className, DateTime date)
    {
        var students = await _students.GetAllAsync();
        var classStudents = students.Where(s => s.Class.Equals(className, StringComparison.OrdinalIgnoreCase)).ToList();

        var records = await _repo.GetByDateAsync(date.Date);

        var result = new List<ClassAttendanceResponse>();

        foreach (var s in classStudents)
        {
            var rec = records.FirstOrDefault(r => r.StudentId == s.Id);

            result.Add(new ClassAttendanceResponse(
                s.Id,
                s.Name,
                s.Class,
                date.Date,
                rec?.Status.ToString() ?? "NotMarked",
                rec?.MarkedBy ?? ""
            ));
        }

        return result;
    }
}