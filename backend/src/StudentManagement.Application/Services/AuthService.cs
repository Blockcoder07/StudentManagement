using Microsoft.Extensions.Logging;
using StudentManagement.Application.Common.Helpers;
using StudentManagement.Application.DTOs.Auth;
using StudentManagement.Application.Interfaces.Authentication;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Application.Interfaces.Services;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;

namespace StudentManagement.Application.Services;

public class AuthService : IAuthService
{
    #region Fields

    private readonly IUserRepository _objUserRepository;
    private readonly IPasswordHasher _objPasswordHasher;
    private readonly IJwtTokenService _objJwtTokenService;
    private readonly ILogger<AuthService> _objLogger;

    #endregion

    #region Constructor

    public AuthService(
        IUserRepository objUserRepository,
        IPasswordHasher objPasswordHasher,
        IJwtTokenService objJwtTokenService,
        ILogger<AuthService> objLogger)
    {
        _objUserRepository = objUserRepository;
        _objPasswordHasher = objPasswordHasher;
        _objJwtTokenService = objJwtTokenService;
        _objLogger = objLogger;
    }

    #endregion

    #region Public Methods

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto objRequest, CancellationToken objCancellationToken = default)
    {
        ApplicationUser? objUser = await _objUserRepository.GetByUsernameAsync(objRequest.Username, objCancellationToken);

        if (objUser is null || !_objPasswordHasher.Verify(objRequest.Password, objUser.PasswordHash, objUser.PasswordSalt))
        {
            _objLogger.LogWarning("Failed login attempt for username '{Username}'.", objRequest.Username);
            throw new UnauthorizedAccessException(ResponseMessage.InvalidCredentials.GetDescription());
        }

        if (!objUser.IsActive)
        {
            _objLogger.LogWarning("Login attempt for inactive user '{Username}'.", objRequest.Username);
            throw new UnauthorizedAccessException(ResponseMessage.AccountInactive.GetDescription());
        }

        (string stToken, DateTime dtExpiresAt) = _objJwtTokenService.GenerateToken(objUser);

        objUser.LastLoginAt = DateTime.UtcNow;
        await _objUserRepository.UpdateAsync(objUser, objCancellationToken);

        _objLogger.LogInformation("User '{Username}' authenticated successfully.", objUser.Username);

        return new AuthResponseDto(
            AccessToken: stToken,
            ExpiresAt: dtExpiresAt,
            Username: objUser.Username,
            Email: objUser.Email,
            Role: objUser.Role.ToString());
    }

    #endregion
}
