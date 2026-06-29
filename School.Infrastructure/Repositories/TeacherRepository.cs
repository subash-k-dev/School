using Microsoft.EntityFrameworkCore;
using School.Application.Interfaces.Repositories;
using School.Domain.Entities;
using School.Infrastructure.Persistence;

namespace School.Infrastructure.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly AppDbContext _db;

    public TeacherRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Teacher>> GetAllAsync()
        => _db.Teachers.AsNoTracking().ToListAsync();

    public Task<Teacher?> GetByIdAsync(Guid id)
        => _db.Teachers.FirstOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(Teacher teacher)
    {
        _db.Teachers.Add(teacher);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Teacher teacher)
    {
        _db.Teachers.Update(teacher);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Teacher teacher)
    {
        _db.Teachers.Remove(teacher);
        await _db.SaveChangesAsync();
    }
}