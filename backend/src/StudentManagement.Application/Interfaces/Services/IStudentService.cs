using StudentManagement.Application.Common.Responses;
using StudentManagement.Application.DTOs.Student;

namespace StudentManagement.Application.Interfaces.Services;

public interface IStudentService
{
    Task<PagedResult<StudentDto>> GetStudentsAsync(StudentQueryParameters objParameters, CancellationToken objCancellationToken = default);

    Task<StudentDto> GetStudentByIdAsync(int inId, CancellationToken objCancellationToken = default);

    Task<StudentDto> CreateStudentAsync(CreateStudentDto objRequest, CancellationToken objCancellationToken = default);

    Task<StudentDto> UpdateStudentAsync(UpdateStudentDto objRequest, CancellationToken objCancellationToken = default);

    Task DeleteStudentAsync(int inId, CancellationToken objCancellationToken = default);
}
