using School.Domain.Entities;

namespace School.Application.Interfaces.Repositories;

public interface ITeacherRepository
{
    Task<List<Teacher>> GetAllAsync();
    Task<Teacher?> GetByIdAsync(Guid id);
    Task AddAsync(Teacher teacher);
    Task UpdateAsync(Teacher teacher);
    Task DeleteAsync(Teacher teacher);
}