using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Students;
using School.Application.Services;
using School.Domain.Security;

namespace School.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // must be logged in
public class StudentsController : ControllerBase
{
    private readonly StudentService _service;

    public StudentsController(StudentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var student = await _service.GetByIdAsync(id);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)] // only Admin can create
    public async Task<IActionResult> Create(CreateStudentRequest req)
        => Ok(await _service.CreateAsync(req));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)] // only Admin can update
    public async Task<IActionResult> Update(Guid id, UpdateStudentRequest req)
    {
        var ok = await _service.UpdateAsync(id, req);
        return ok ? Ok(new { Message = "Updated" }) : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)] // only Admin can delete
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? Ok(new { Message = "Deleted" }) : NotFound();
    }
}