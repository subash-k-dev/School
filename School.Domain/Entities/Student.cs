using System.Linq;
using School.Domain.Entities.Enums;

namespace School.Domain.Entities;

public class Student
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // ✅ New clean fields (use for new Students module)
    public string Rollnumber { get; set; } = string.Empty;
    public string AdmissionNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
    public Gender Gender { get; set; } = Gender.Other;
    public string ClassName { get; set; } = string.Empty; // ex: "5A"
    public StudentStatus Status { get; set; } = StudentStatus.Active;
    public decimal TotalFee { get; set; } = 0;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    // ✅ Computed Age
    public int CalculatedAge
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return Math.Max(age, 0);
        }
    }

    // -----------------------------------------
    // 🔁 Old fields kept so nothing breaks today
    // -----------------------------------------

    [Obsolete("Use FirstName + LastName instead. Will be removed later.")]
    public string Name
    {
        get
        {
            var full = $"{FirstName} {LastName}".Trim();
            return string.IsNullOrWhiteSpace(full) ? string.Empty : full;
        }
        set
        {
            value ??= string.Empty;
            var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                FirstName = string.Empty;
                LastName = string.Empty;
            }
            else if (parts.Length == 1)
            {
                FirstName = parts[0];
                LastName = string.Empty;
            }
            else
            {
                FirstName = parts[0];
                LastName = string.Join(' ', parts.Skip(1));
            }
        }
    }

    [Obsolete("Use ClassName instead. Will be removed later.")]
    public string Class
    {
        get => ClassName;
        set => ClassName = value ?? string.Empty;
    }

    [Obsolete("Use DateOfBirth/CalculatedAge instead. Will be removed later.")]
    public int Age
    {
        get => CalculatedAge;
        set
        {
            var years = Math.Clamp(value, 0, 120);
            DateOfBirth = DateTime.SpecifyKind(
    new DateTime(DateTime.Today.Year - years, 1, 1),
    DateTimeKind.Utc
);
        }
    }
}