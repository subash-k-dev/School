using Microsoft.EntityFrameworkCore;
using School.Application.Interfaces.Repositories;
using School.Domain.Entities;
using School.Infrastructure.Persistence;

namespace School.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _db;

    public StudentRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Student>> GetAllAsync()
        => _db.Students.AsNoTracking().ToListAsync();

    public Task<Student?> GetByIdAsync(Guid id)
        => _db.Students.FirstOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(Student student)
    {
        _db.Students.Add(student);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        _db.Students.Update(student);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Student student)
    {
        _db.Students.Remove(student);
        await _db.SaveChangesAsync();
    }
}