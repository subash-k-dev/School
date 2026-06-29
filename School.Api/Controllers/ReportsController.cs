using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.Services;

namespace School.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reports;

    public ReportsController(ReportService reports)
    {
        _reports = reports;
    }

    // 1) Attendance report by class + date range
    [HttpGet("attendance")]
    public async Task<IActionResult> Attendance(
        [FromQuery] string className,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var result = await _reports.GetAttendanceReportAsync(className, fromDate, toDate);
        return Ok(result);
    }

    // 2) Student profile report
    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> StudentProfile(Guid studentId)
    {
        var result = await _reports.GetStudentProfileAsync(studentId);
        return result is null ? NotFound("Student not found") : Ok(result);
    }

    [HttpGet("student/by-roll/{rollNumber}")]
    public async Task<IActionResult> StudentProfileByRoll(string rollNumber)
    {
        var result = await _reports.GetStudentProfileByRollAsync(rollNumber);
        return result is null ? NotFound("Student not found") : Ok(result);
    }

    // 3) Fee due report
    [HttpGet("fees/dues")]
    public async Task<IActionResult> FeeDues([FromQuery] string? className = null)
        => Ok(await _reports.GetFeeDueReportAsync(className));

    // 4) Download Fee Due Report as Excel
    [HttpGet("fees/dues/excel")]
    public async Task<IActionResult> FeeDuesExcel([FromQuery] string? className = null)
    {
        var rows = await _reports.GetFeeDueReportAsync(className);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("FeeDueReport");

        ws.Cell(1, 1).Value = "Student Id";
        ws.Cell(1, 2).Value = "Student Name";
        ws.Cell(1, 3).Value = "Class";
        ws.Cell(1, 4).Value = "Total Fee";
        ws.Cell(1, 5).Value = "Paid Amount";
        ws.Cell(1, 6).Value = "Due Amount";

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            int r = i + 2;

            ws.Cell(r, 1).Value = row.StudentId.ToString();
            ws.Cell(r, 2).Value = row.StudentName;
            ws.Cell(r, 3).Value = row.Class;
            ws.Cell(r, 4).Value = row.TotalFee;
            ws.Cell(r, 5).Value = row.PaidAmount;
            ws.Cell(r, 6).Value = row.DueAmount;
        }

        ws.Columns().AdjustToContents();
        ws.Row(1).Style.Font.Bold = true;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var fileName = string.IsNullOrWhiteSpace(className)
            ? $"FeeDueReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            : $"FeeDueReport_{className}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    // 5) Download Attendance Report as Excel
    [HttpGet("attendance/excel")]
    public async Task<IActionResult> AttendanceExcel(
        [FromQuery] string className,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var report = await _reports.GetAttendanceReportAsync(className, fromDate, toDate);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("AttendanceReport");

        ws.Cell(1, 1).Value = "Class";
        ws.Cell(1, 2).Value = report.Class;
        ws.Cell(2, 1).Value = "From Date";
        ws.Cell(2, 2).Value = report.FromDate.ToString("yyyy-MM-dd");
        ws.Cell(3, 1).Value = "To Date";
        ws.Cell(3, 2).Value = report.ToDate.ToString("yyyy-MM-dd");

        ws.Cell(5, 1).Value = "Student Id";
        ws.Cell(5, 2).Value = "Student Name";
        ws.Cell(5, 3).Value = "Class";
        ws.Cell(5, 4).Value = "Date";
        ws.Cell(5, 5).Value = "Status";
        ws.Cell(5, 6).Value = "Marked By";

        for (int i = 0; i < report.Items.Count; i++)
        {
            var item = report.Items[i];
            int r = i + 6;

            ws.Cell(r, 1).Value = item.StudentId.ToString();
            ws.Cell(r, 2).Value = item.StudentName;
            ws.Cell(r, 3).Value = item.Class;
            ws.Cell(r, 4).Value = item.Date.ToString("yyyy-MM-dd");
            ws.Cell(r, 5).Value = item.Status;
            ws.Cell(r, 6).Value = item.MarkedBy;
        }

        ws.Columns().AdjustToContents();
        ws.Row(5).Style.Font.Bold = true;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"AttendanceReport_{className}_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    // 6) Download Student Profile Report as Excel
    [HttpGet("student/{studentId:guid}/excel")]
    public async Task<IActionResult> StudentProfileExcel(Guid studentId)
    {
        var report = await _reports.GetStudentProfileAsync(studentId);
        if (report is null)
            return NotFound("Student not found");

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("StudentProfile");

        ws.Cell(1, 1).Value = "Student Id";
        ws.Cell(1, 2).Value = report.StudentId.ToString();

        ws.Cell(2, 1).Value = "Student Name";
        ws.Cell(2, 2).Value = report.StudentName;

        ws.Cell(3, 1).Value = "Class";
        ws.Cell(3, 2).Value = report.Class;

        ws.Cell(4, 1).Value = "Age";
        ws.Cell(4, 2).Value = report.Age;

        ws.Cell(5, 1).Value = "Total Fee";
        ws.Cell(5, 2).Value = report.TotalFee;

        ws.Cell(6, 1).Value = "Paid Amount";
        ws.Cell(6, 2).Value = report.PaidAmount;

        ws.Cell(7, 1).Value = "Due Amount";
        ws.Cell(7, 2).Value = report.DueAmount;

        ws.Cell(8, 1).Value = "Present Count";
        ws.Cell(8, 2).Value = report.PresentCount;

        ws.Cell(9, 1).Value = "Absent Count";
        ws.Cell(9, 2).Value = report.AbsentCount;

        ws.Cell(10, 1).Value = "Not Marked Count";
        ws.Cell(10, 2).Value = report.NotMarkedCount;

        ws.Columns().AdjustToContents();
        ws.Column(1).Style.Font.Bold = true;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"StudentProfile_{report.StudentName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpGet("student/by-roll/{rollNumber}/excel")]
    public async Task<IActionResult> StudentProfileByRollExcel(string rollNumber)
    {
        var report = await _reports.GetStudentProfileByRollAsync(rollNumber);
        if (report is null)
            return NotFound("Student not found");

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("StudentProfile");

        ws.Cell(1, 1).Value = "Student Id";
        ws.Cell(1, 2).Value = report.StudentId.ToString();

        ws.Cell(2, 1).Value = "Student Name";
        ws.Cell(2, 2).Value = report.StudentName;

        ws.Cell(3, 1).Value = "Class";
        ws.Cell(3, 2).Value = report.Class;

        ws.Cell(4, 1).Value = "Age";
        ws.Cell(4, 2).Value = report.Age;

        ws.Cell(5, 1).Value = "Total Fee";
        ws.Cell(5, 2).Value = report.TotalFee;

        ws.Cell(6, 1).Value = "Paid Amount";
        ws.Cell(6, 2).Value = report.PaidAmount;

        ws.Cell(7, 1).Value = "Due Amount";
        ws.Cell(7, 2).Value = report.DueAmount;

        ws.Cell(8, 1).Value = "Present Count";
        ws.Cell(8, 2).Value = report.PresentCount;

        ws.Cell(9, 1).Value = "Absent Count";
        ws.Cell(9, 2).Value = report.AbsentCount;

        ws.Cell(10, 1).Value = "Not Marked Count";
        ws.Cell(10, 2).Value = report.NotMarkedCount;

        ws.Columns().AdjustToContents();
        ws.Column(1).Style.Font.Bold = true;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var safeRoll = string.Concat(rollNumber.Split(Path.GetInvalidFileNameChars()));
        var fileName = $"StudentProfile_{safeRoll}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

}