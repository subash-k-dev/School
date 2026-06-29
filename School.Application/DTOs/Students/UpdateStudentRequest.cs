namespace School.Application.DTOs.Students;

public record UpdateStudentRequest(string Rollnumber, string Name, int Age, string Class, decimal TotalFee);