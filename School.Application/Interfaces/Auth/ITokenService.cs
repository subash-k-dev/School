using School.Domain.Entities;

namespace School.Application.Interfaces.Auth;

public interface ITokenService
{
    string GenerateToken(User user);
}