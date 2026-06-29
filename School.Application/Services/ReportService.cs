using School.Application.DTOs.Reports;
using School.Application.Interfaces.Repositories;
using School.Domain.Entities.Enums;

namespace School.Application.Services;

public class ReportService
{
    private readonly IStudentRepository _students;
    private readonly IFeePaymentRepository _fees;
    private readonly IAttendanceRepository _attendance;

    public ReportService(
        IStudentRepository students,
        IFeePaymentRepository fees,
        IAttendanceRepository attendance)
    {
        _students = students;
        _fees = fees;
        _attendance = attendance;
    }

    private static DateTime DateOnly(DateTime d) => d.Date;

    // 1) Attendance report by class + date range
    public async Task<AttendanceReportResponse> GetAttendanceReportAsync(string className, DateTime fromDate, DateTime toDate)
    {
        fromDate = DateOnly(fromDate);
        toDate = DateOnly(toDate);

        if (toDate < fromDate)
            throw new ArgumentException("toDate must be >= fromDate");

        var allStudents = await _students.GetAllAsync();
        var classStudents = allStudents
            .Where(s => s.Class.Equals(className, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Collect records for each date in range
        var items = new List<AttendanceReportItem>();

        for (var date = fromDate; date <= toDate; date = date.AddDays(1))
        {
            var dayRecords = await _attendance.GetByDateAsync(date);

            foreach (var s in classStudents)
            {
                var rec = dayRecords.FirstOrDefault(r => r.StudentId == s.Id);

                items.Add(new AttendanceReportItem(
                    s.Id,
                    s.Name,
                    s.Class,
                    date,
                    rec?.Status.ToString() ?? "NotMarked",
                    rec?.MarkedBy ?? ""
                ));
            }
        }

        return new AttendanceReportResponse(className, fromDate, toDate, items);
    }

    // 2) Student profile report
    public async Task<StudentProfileReportResponse?> GetStudentProfileAsync(Guid studentId)
    {
        var student = await _students.GetByIdAsync(studentId);
        if (student is null) return null;

        var paid = await _fees.GetTotalPaidByStudentAsync(studentId);
        var due = Math.Max(0, student.TotalFee - paid);

        var history = await _attendance.GetByStudentAsync(studentId);

        var present = history.Count(x => x.Status == AttendanceStatus.Present);
        var absent = history.Count(x => x.Status == AttendanceStatus.Absent);

        // For NotMarked: we don’t know total days unless we define a date range.
        // For now keep it as 0 (we can improve later with date range).
        var notMarked = 0;

        return new StudentProfileReportResponse(
            student.Id,
            student.Name,
            student.Class,
            student.Age,
            student.TotalFee,
            paid,
            due,
            present,
            absent,
            notMarked
        );
    }

    // 3) Fee due report (optional class filter)
    public async Task<List<FeeDueReportItem>> GetFeeDueReportAsync(string? className = null)
    {
        var students = await _students.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(className))
        {
            students = students
                .Where(s => s.Class.Equals(className, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var list = new List<FeeDueReportItem>();

        foreach (var s in students)
        {
            var paid = await _fees.GetTotalPaidByStudentAsync(s.Id);
            var due = Math.Max(0, s.TotalFee - paid);

            if (due > 0)
            {
                list.Add(new FeeDueReportItem(
                    s.Id,
                    s.Name,
                    s.Class,
                    s.TotalFee,
                    paid,
                    due
                ));
            }
        }

        return list.OrderByDescending(x => x.DueAmount).ToList();
    }

    public async Task<StudentProfileReportResponse?> GetStudentProfileByRollAsync(string rollNumber)
    {
        var allStudents = await _students.GetAllAsync();

        var student = allStudents.FirstOrDefault(s =>
            string.Equals(s.Rollnumber, rollNumber, StringComparison.OrdinalIgnoreCase));

        if (student is null) return null;

        var paid = await _fees.GetTotalPaidByStudentAsync(student.Id);
        var due = Math.Max(0, student.TotalFee - paid);

        var history = await _attendance.GetByStudentAsync(student.Id);

        var present = history.Count(x => x.Status == AttendanceStatus.Present);
        var absent = history.Count(x => x.Status == AttendanceStatus.Absent);
        var notMarked = 0;

        return new StudentProfileReportResponse(
            student.Id,
            student.Name,
            student.Class,
            student.Age,
            student.TotalFee,
            paid,
            due,
            present,
            absent,
            notMarked
        );
    }
}