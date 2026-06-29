namespace School.Api.Models;

public class ErrorResponse
{
    public string Message { get; set; } = "An error occurred";
    public string? Details { get; set; }
    public string TraceId { get; set; } = string.Empty;
}