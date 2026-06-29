using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Fees;
using School.Application.Services;
using School.Domain.Security;
using School.Application.Interfaces.Repositories;

namespace School.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeesController : ControllerBase
{
    private readonly FeeService _service;
    private readonly IFeePaymentRepository _repo;

    public FeesController(FeeService service, IFeePaymentRepository repo)
    {
        _service = service;
        _repo = repo;
    }

    // Admin records payment
    [HttpPost("pay")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Pay(PayFeeRequest req)
    {
        var result = await _service.PayAsync(req);
        return result is null ? NotFound("Student not found") : Ok(result);
    }

    // View payment history for a student (Admin + Teacher)
    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> StudentPayments(Guid studentId)
    {
        var payments = await _service.GetStudentPaymentsAsync(studentId);
        return payments is null ? NotFound("Student not found") : Ok(payments);
    }

    // View fee summary for a student (Admin + Teacher)
    [HttpGet("student/{studentId:guid}/summary")]
    public async Task<IActionResult> StudentSummary(Guid studentId)
    {
        var summary = await _service.GetStudentSummaryAsync(studentId);
        return summary is null ? NotFound("Student not found") : Ok(summary);
    }

    // Due list report (Admin + Teacher)
    [HttpGet("dues")]
    public async Task<IActionResult> Dues()
        => Ok(await _service.GetDuesAsync());

    // ✅ Dashboard count (Admin + Teacher)
    [HttpGet]
    public async Task<IActionResult> GetCount()
    {
        // easiest: count dues list OR count total payments if you have such method later
        var dues = await _service.GetDuesAsync();
        return Ok(new { count = dues?.Count ?? 0 });
    }
    
}