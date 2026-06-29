namespace School.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserName { get; set; } = string.Empty;

    // For now we store plain password in PasswordHash to make it working fast.
    // Next step we will convert to real hashing.
    public string PasswordHash { get; set; } = string.Empty;

    // "Admin" or "Teacher"
    public string Role { get; set; } = "Teacher";
}