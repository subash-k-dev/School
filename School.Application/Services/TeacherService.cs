using School.Application.DTOs.Teachers;
using School.Application.Interfaces.Repositories;
using School.Domain.Entities;

namespace School.Application.Services;

public class TeacherService
{
    private readonly ITeacherRepository _repo;

    public TeacherService(ITeacherRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TeacherResponse>> GetAllAsync()
    {
        var teachers = await _repo.GetAllAsync();
        return teachers.Select(t => new TeacherResponse(t.Id, t.Name, t.Subject, t.Phone)).ToList();
    }

    public async Task<TeacherResponse?> GetByIdAsync(Guid id)
    {
        var t = await _repo.GetByIdAsync(id);
        return t is null ? null : new TeacherResponse(t.Id, t.Name, t.Subject, t.Phone);
    }

    public async Task<TeacherResponse> CreateAsync(CreateTeacherRequest req)
    {
        var teacher = new Teacher
        {
            Name = req.Name,
            Subject = req.Subject,
            Phone = req.Phone
        };

        await _repo.AddAsync(teacher);
        return new TeacherResponse(teacher.Id, teacher.Name, teacher.Subject, teacher.Phone);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTeacherRequest req)
    {
        var teacher = await _repo.GetByIdAsync(id);
        if (teacher is null) return false;

        teacher.Name = req.Name;
        teacher.Subject = req.Subject;
        teacher.Phone = req.Phone;

        await _repo.UpdateAsync(teacher);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var teacher = await _repo.GetByIdAsync(id);
        if (teacher is null) return false;

        await _repo.DeleteAsync(teacher);
        return true;
    }
}