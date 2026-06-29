using School.Domain.Entities;

namespace School.Application.Interfaces.Repositories;

public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(Guid id);
    Task AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(Student student);
}