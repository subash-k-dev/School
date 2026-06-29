using Microsoft.AspNetCore.Identity;
using School.Application.Interfaces.Auth;
using School.Domain.Entities;

namespace School.Infrastructure.Auth;

public class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password)
    {
        // User object required by API, but not used by default hasher logic
        var dummyUser = new User();
        return _hasher.HashPassword(dummyUser, password);
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {
        var dummyUser = new User();

        var result = _hasher.VerifyHashedPassword(dummyUser, hashedPassword, providedPassword);

        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}