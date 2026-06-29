using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Attendance;
using School.Application.Interfaces.Repositories;
using School.Application.Services;
using System.Security.Claims;

namespace School.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly AttendanceService _service;
    private readonly IAttendanceRepository _repo;

    public AttendanceController(AttendanceService service, IAttendanceRepository repo)
    {
        _service = service;
        _repo = repo;
    }

    [HttpPost("mark")]
    public async Task<IActionResult> Mark([FromBody] MarkAttendanceRequest req)
    {
        var markedBy = User.FindFirstValue(ClaimTypes.Name) ?? "unknown";

        var result = await _service.MarkAsync(req, markedBy);

        return result is null ? NotFound("Student not found") : Ok(result);
    }

    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> StudentHistory(Guid studentId)
    {
        var list = await _service.GetStudentHistoryAsync(studentId);
        return list is null ? NotFound("Student not found") : Ok(list);
    }

    [HttpGet("class/{className}/date/{date}")]
    public async Task<IActionResult> ClassAttendance(string className, string date)
    {
        if (!DateTime.TryParse(date, out var dt))
            return BadRequest("Date must be yyyy-MM-dd");

        var result = await _service.GetClassAttendanceAsync(className, dt.Date);
        return Ok(result);
    }

    // ✅ Dashboard count (today's attendance records)
    [HttpGet]
    public async Task<IActionResult> GetCount()
    {
        var today = DateTime.UtcNow.Date;

        var list = await _repo.GetByDateAsync(today);

        return Ok(new { count = list.Count });
    }
}