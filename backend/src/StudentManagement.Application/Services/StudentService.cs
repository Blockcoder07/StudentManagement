using AutoMapper;
using Microsoft.Extensions.Logging;
using StudentManagement.Application.Common.Exceptions;
using StudentManagement.Application.Common.Helpers;
using StudentManagement.Application.Common.Responses;
using StudentManagement.Application.DTOs.Student;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Application.Interfaces.Services;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;

namespace StudentManagement.Application.Services;

public class StudentService : IStudentService
{
    #region Fields

    private readonly IStudentRepository _objStudentRepository;
    private readonly IMapper _objMapper;
    private readonly ILogger<StudentService> _objLogger;

    #endregion

    #region Constructor

    public StudentService(
        IStudentRepository objStudentRepository,
        IMapper objMapper,
        ILogger<StudentService> objLogger)
    {
        _objStudentRepository = objStudentRepository;
        _objMapper = objMapper;
        _objLogger = objLogger;
    }

    #endregion

    #region Public Methods

    public async Task<PagedResult<StudentDto>> GetStudentsAsync(
        StudentQueryParameters objParameters,
        CancellationToken objCancellationToken = default)
    {
        (IReadOnlyList<Student> lstItems, int inTotal) = await _objStudentRepository.GetPagedAsync(objParameters, objCancellationToken);

        return new PagedResult<StudentDto>
        {
            Items = lstItems.Select(_objMapper.Map<StudentDto>).ToList(),
            TotalCount = inTotal,
            PageNumber = objParameters.PageNumber,
            PageSize = objParameters.PageSize
        };
    }

    public async Task<StudentDto> GetStudentByIdAsync(int inId, CancellationToken objCancellationToken = default)
    {
        Student objStudent = await _objStudentRepository.GetByIdAsync(inId, objCancellationToken)
            ?? throw new NotFoundException(ResponseMessage.StudentNotFound.GetDescription());

        return _objMapper.Map<StudentDto>(objStudent);
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto objRequest, CancellationToken objCancellationToken = default)
    {
        bool isEmailExists = await _objStudentRepository.EmailExistsAsync(objRequest.Email, objCancellationToken: objCancellationToken);
        if (isEmailExists)
        {
            throw new ConflictException(ResponseMessage.StudentEmailExists.GetDescription());
        }

        Student objEntity = _objMapper.Map<Student>(objRequest);
        objEntity.Id = 0;
        objEntity.IsActive = true;

        Student objSaved = await _objStudentRepository.SaveAsync(objEntity, objCancellationToken);

        _objLogger.LogInformation("Student {StudentId} ({Email}) created via usp_Student_Save.", objSaved.Id, objSaved.Email);
        return _objMapper.Map<StudentDto>(objSaved);
    }

    public async Task<StudentDto> UpdateStudentAsync(UpdateStudentDto objRequest, CancellationToken objCancellationToken = default)
    {
        Student objExisting = await _objStudentRepository.GetByIdAsync(objRequest.Id, objCancellationToken)
            ?? throw new NotFoundException(ResponseMessage.StudentNotFound.GetDescription());

        bool isEmailTaken = await _objStudentRepository.EmailExistsAsync(objRequest.Email, objRequest.Id, objCancellationToken);
        if (isEmailTaken)
        {
            throw new ConflictException(ResponseMessage.StudentEmailExists.GetDescription());
        }

        objExisting.Name = objRequest.Name;
        objExisting.Email = objRequest.Email;
        objExisting.Age = objRequest.Age;
        objExisting.Course = objRequest.Course;
        objExisting.IsActive = objRequest.IsActive;

        Student objSaved = await _objStudentRepository.SaveAsync(objExisting, objCancellationToken);

        _objLogger.LogInformation("Student {StudentId} updated via usp_Student_Save.", objSaved.Id);
        return _objMapper.Map<StudentDto>(objSaved);
    }

    public async Task DeleteStudentAsync(int inId, CancellationToken objCancellationToken = default)
    {
        bool isDeleted = await _objStudentRepository.DeleteAsync(inId, objCancellationToken);
        if (!isDeleted)
        {
            throw new NotFoundException(ResponseMessage.StudentNotFound.GetDescription());
        }

        _objLogger.LogInformation("Student {StudentId} deleted via usp_Student_Delete.", inId);
    }

    #endregion
}
