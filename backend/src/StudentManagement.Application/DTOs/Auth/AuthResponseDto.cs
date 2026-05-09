namespace StudentManagement.Application.DTOs.Auth;

public record AuthResponseDto(
    string AccessToken,
    DateTime ExpiresAt,
    string Username,
    string Email,
    string Role);
