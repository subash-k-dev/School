using School.Domain.Entities;

namespace School.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string userName);
    Task AddAsync(User user);
}