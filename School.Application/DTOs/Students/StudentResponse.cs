namespace School.Application.DTOs.Students;

public record StudentResponse(
    Guid Id,
    string Rollnumber,
    string Name,
    int Age,
    string Class,
    decimal TotalFee
);