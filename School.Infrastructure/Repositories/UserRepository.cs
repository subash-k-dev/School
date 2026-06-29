using Microsoft.EntityFrameworkCore;
using School.Application.Interfaces.Repositories;
using School.Domain.Entities;
using School.Infrastructure.Persistence;

namespace School.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByUserNameAsync(string userName)
        => _db.Users.FirstOrDefaultAsync(x => x.UserName == userName);

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
}