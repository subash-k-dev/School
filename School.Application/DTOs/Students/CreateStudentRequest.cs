namespace School.Application.DTOs.Students;

public record CreateStudentRequest(string Rollnumber, string Name, int Age, string Class, decimal TotalFee);