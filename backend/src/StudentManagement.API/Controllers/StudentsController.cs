using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.Common.Helpers;
using StudentManagement.Application.Common.Responses;
using StudentManagement.Application.DTOs.Student;
using StudentManagement.Application.Interfaces.Services;
using StudentManagement.Domain.Enums;

namespace StudentManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    #region Fields

    private readonly IStudentService _objStudentService;

    #endregion

    #region Constructor

    public StudentsController(IStudentService objStudentService)
    {
        _objStudentService = objStudentService;
    }

    #endregion

    #region Endpoints

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StudentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] StudentQueryParameters objParameters,
        CancellationToken objCancellationToken)
    {
        PagedResult<StudentDto> objResult = await _objStudentService.GetStudentsAsync(objParameters, objCancellationToken);
        return Ok(ApiResponse<PagedResult<StudentDto>>.Ok(
            objResult,
            ResponseMessage.StudentsFetched.GetDescription()));
    }

    [HttpGet("{inId:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int inId, CancellationToken objCancellationToken)
    {
        StudentDto objResult = await _objStudentService.GetStudentByIdAsync(inId, objCancellationToken);
        return Ok(ApiResponse<StudentDto>.Ok(
            objResult,
            ResponseMessage.StudentFetched.GetDescription()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateStudentDto objRequest,
        CancellationToken objCancellationToken)
    {
        StudentDto objResult = await _objStudentService.CreateStudentAsync(objRequest, objCancellationToken);
        ApiResponse<StudentDto> objResponse = ApiResponse<StudentDto>.Ok(
            objResult,
            ResponseMessage.StudentCreated.GetDescription(),
            StatusCodes.Status201Created);

        return CreatedAtAction(nameof(GetById), new { inId = objResult.Id }, objResponse);
    }

    [HttpPut("{inId:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        int inId,
        [FromBody] UpdateStudentDto objRequest,
        CancellationToken objCancellationToken)
    {
        if (inId != objRequest.Id)
        {
            return BadRequest(ApiResponse.Fail(
                "Route id does not match the payload id.",
                StatusCodes.Status400BadRequest));
        }

        StudentDto objResult = await _objStudentService.UpdateStudentAsync(objRequest, objCancellationToken);
        return Ok(ApiResponse<StudentDto>.Ok(
            objResult,
            ResponseMessage.StudentUpdated.GetDescription()));
    }

    [HttpDelete("{inId:int}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int inId, CancellationToken objCancellationToken)
    {
        await _objStudentService.DeleteStudentAsync(inId, objCancellationToken);
        return Ok(ApiResponse.Ok(ResponseMessage.StudentDeleted.GetDescription()));
    }

    #endregion
}
