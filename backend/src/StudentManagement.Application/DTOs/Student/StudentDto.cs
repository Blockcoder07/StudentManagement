namespace StudentManagement.Application.DTOs.Student;

public record StudentDto(
    int Id,
    string Name,
    string Email,
    int Age,
    string Course,
    DateTime CreatedDate,
    DateTime? ModifiedDate,
    bool IsActive);
