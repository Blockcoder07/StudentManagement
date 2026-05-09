using StudentManagement.Application.DTOs.Auth;

namespace StudentManagement.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto objRequest, CancellationToken objCancellationToken = default);
}
