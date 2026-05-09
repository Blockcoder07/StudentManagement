namespace StudentManagement.Application.DTOs.Student;

public record CreateStudentDto(
    string Name,
    string Email,
    int Age,
    string Course);
