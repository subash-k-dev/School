namespace School.Application.Interfaces.Auth;

public interface IPasswordHasherService
{
    string Hash(string password);
    bool Verify(string hashedPassword, string providedPassword);
}