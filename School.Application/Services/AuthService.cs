using School.Application.DTOs.Auth;
using School.Application.Interfaces.Auth;
using School.Application.Interfaces.Repositories;

namespace School.Application.Services;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;
    private readonly IPasswordHasherService _hasher;

    public AuthService(IUserRepository users, ITokenService tokens, IPasswordHasherService hasher)
    {
        _users = users;
        _tokens = tokens;
        _hasher = hasher;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest req)
    {
        var user = await _users.GetByUserNameAsync(req.UserName);
        if (user is null) return null;

        // ✅ Verify hash instead of plain string compare
        if (!_hasher.Verify(user.PasswordHash, req.Password))
            return null;

        var token = _tokens.GenerateToken(user);
        return new LoginResponse(token, user.Role);
    }
}