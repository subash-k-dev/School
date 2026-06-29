using School.Application.DTOs.Students;
using School.Application.Interfaces.Repositories;
using School.Domain.Entities;

namespace School.Application.Services;

public class StudentService
{
    private readonly IStudentRepository _repo;

    public StudentService(IStudentRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<StudentResponse>> GetAllAsync()
    {
        var students = await _repo.GetAllAsync();
        return students.Select(s => new StudentResponse(s.Id, s.Rollnumber, s.Name, s.Age, s.Class, s.TotalFee)).ToList();
    }

    public async Task<StudentResponse?> GetByIdAsync(Guid id)
    {
        var s = await _repo.GetByIdAsync(id);
        return s is null ? null : new StudentResponse(s.Id, s.Rollnumber, s.Name, s.Age, s.Class, s.TotalFee);
    }

    public async Task<StudentResponse> CreateAsync(CreateStudentRequest req)
    {
        var student = new Student
        {
            Name = req.Name,
            Age = req.Age,
            Class = req.Class,
            TotalFee = req.TotalFee
        };

        await _repo.AddAsync(student);

        return new StudentResponse(student.Id, student.Rollnumber, student.Name, student.Age, student.Class, student.TotalFee);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateStudentRequest req)
    {
        var student = await _repo.GetByIdAsync(id);
        if (student is null) return false;

        student.Name = req.Name;
        student.Age = req.Age;
        student.Class = req.Class;

        await _repo.UpdateAsync(student);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var student = await _repo.GetByIdAsync(id);
        if (student is null) return false;

        await _repo.DeleteAsync(student);
        return true;
    }
}