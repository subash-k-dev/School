namespace School.Domain.Entities;

public class Teacher
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;   // ex: "Math"

    public string Phone { get; set; } = string.Empty;     // optional
}