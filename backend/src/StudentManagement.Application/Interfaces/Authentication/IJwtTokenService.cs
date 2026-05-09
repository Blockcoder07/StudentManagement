using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Interfaces.Authentication;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser objUser);
}
