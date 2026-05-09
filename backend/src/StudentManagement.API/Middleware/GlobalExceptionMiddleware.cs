using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.Data.SqlClient;
using StudentManagement.Application.Common.Exceptions;
using StudentManagement.Application.Common.Helpers;
using StudentManagement.Application.Common.Responses;
using StudentManagement.Domain.Enums;

namespace StudentManagement.API.Middleware;

public class GlobalExceptionMiddleware
{
    #region Fields

    private static readonly JsonSerializerOptions ObjSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _objNext;
    private readonly ILogger<GlobalExceptionMiddleware> _objLogger;

    #endregion

    #region Constructor

    public GlobalExceptionMiddleware(RequestDelegate objNext, ILogger<GlobalExceptionMiddleware> objLogger)
    {
        _objNext = objNext;
        _objLogger = objLogger;
    }

    #endregion

    #region Public Methods

    public async Task InvokeAsync(HttpContext objContext)
    {
        try
        {
            await _objNext(objContext);
        }
        catch (Exception exMessage)
        {
            await HandleAsync(objContext, exMessage);
        }
    }

    #endregion

    #region Private Methods

    private async Task HandleAsync(HttpContext objContext, Exception exMessage)
    {
        (int inStatusCode, ApiResponse objResponse) = MapException(exMessage);

        if (inStatusCode >= 500)
        {
            _objLogger.LogError(exMessage, "Unhandled exception while processing {Method} {Path}",
                objContext.Request.Method, objContext.Request.Path);
        }
        else
        {
            _objLogger.LogWarning("Handled exception ({StatusCode}) for {Method} {Path}: {Message}",
                inStatusCode, objContext.Request.Method, objContext.Request.Path, exMessage.Message);
        }

        objContext.Response.ContentType = "application/json";
        objContext.Response.StatusCode = inStatusCode;

        string stPayload = JsonSerializer.Serialize(objResponse, ObjSerializerOptions);
        await objContext.Response.WriteAsync(stPayload);
    }

    private static (int StatusCode, ApiResponse Response) MapException(Exception exMessage)
    {
        switch (exMessage)
        {
            case ValidationException exValidation:
                {
                    List<string> lstErrors = exValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    ApiResponse objResponse = ApiResponse.Fail(
                        ResponseMessage.ValidationFailed.GetDescription(),
                        (int)HttpStatusCode.BadRequest,
                        lstErrors);
                    return ((int)HttpStatusCode.BadRequest, objResponse);
                }

            case NotFoundException exNotFound:
                return ((int)HttpStatusCode.NotFound,
                    ApiResponse.Fail(exNotFound.Message, (int)HttpStatusCode.NotFound));

            case ConflictException exConflict:
                return ((int)HttpStatusCode.Conflict,
                    ApiResponse.Fail(exConflict.Message, (int)HttpStatusCode.Conflict));

            case UnauthorizedAccessException exUnauthorized:
                return ((int)HttpStatusCode.Unauthorized,
                    ApiResponse.Fail(exUnauthorized.Message, (int)HttpStatusCode.Unauthorized));

            case SqlException exSql:
                return ((int)HttpStatusCode.InternalServerError,
                    ApiResponse.Fail(
                        ResponseMessage.DatabaseError.GetDescription(),
                        (int)HttpStatusCode.InternalServerError,
                        new[] { exSql.Message }));

            default:
                return ((int)HttpStatusCode.InternalServerError,
                    ApiResponse.Fail(
                        ResponseMessage.InternalServerError.GetDescription(),
                        (int)HttpStatusCode.InternalServerError));
        }
    }

    #endregion
}
