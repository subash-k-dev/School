using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Teachers;
using School.Application.Services;
using School.Domain.Security;

namespace School.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly TeacherService _service;

    public TeachersController(TeacherService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var teacher = await _service.GetByIdAsync(id);
        return teacher is null ? NotFound() : Ok(teacher);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create(CreateTeacherRequest req)
        => Ok(await _service.CreateAsync(req));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, UpdateTeacherRequest req)
    {
        var ok = await _service.UpdateAsync(id, req);
        return ok ? Ok(new { Message = "Updated" }) : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? Ok(new { Message = "Deleted" }) : NotFound();
    }
}