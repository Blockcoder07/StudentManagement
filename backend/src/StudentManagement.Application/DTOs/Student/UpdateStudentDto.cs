namespace StudentManagement.Application.DTOs.Student;

public record UpdateStudentDto(
    int Id,
    string Name,
    string Email,
    int Age,
    string Course,
    bool IsActive);
