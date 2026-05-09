using StudentManagement.Application.DTOs.Student;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Interfaces.Repositories;

public interface IStudentRepository
{
    Task<(IReadOnlyList<Student> Items, int TotalCount)> GetPagedAsync(
        StudentQueryParameters objParameters,
        CancellationToken objCancellationToken = default);

    Task<Student?> GetByIdAsync(int inId, CancellationToken objCancellationToken = default);

    Task<bool> EmailExistsAsync(string stEmail, int? inExcludeId = null, CancellationToken objCancellationToken = default);

    Task<Student> SaveAsync(Student objStudent, CancellationToken objCancellationToken = default);

    Task<bool> DeleteAsync(int inId, CancellationToken objCancellationToken = default);
}
