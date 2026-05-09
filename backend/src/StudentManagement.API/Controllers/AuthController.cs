using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.Common.Helpers;
using StudentManagement.Application.Common.Responses;
using StudentManagement.Application.DTOs.Auth;
using StudentManagement.Application.Interfaces.Services;
using StudentManagement.Domain.Enums;

namespace StudentManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    #region Fields

    private readonly IAuthService _objAuthService;

    #endregion

    #region Constructor

    public AuthController(IAuthService objAuthService)
    {
        _objAuthService = objAuthService;
    }

    #endregion

    #region Endpoints

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto objRequest,
        CancellationToken objCancellationToken)
    {
        AuthResponseDto objResult = await _objAuthService.LoginAsync(objRequest, objCancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(
            objResult,
            ResponseMessage.LoginSuccess.GetDescription()));
    }

    #endregion
}
