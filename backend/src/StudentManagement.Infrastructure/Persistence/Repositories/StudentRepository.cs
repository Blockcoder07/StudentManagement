using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.DTOs.Student;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Infrastructure.Persistence.Repositories;

public class StudentRepository : IStudentRepository
{
    #region Constants

    private const string StSpGetList     = "dbo.usp_Student_GetList";
    private const string StSpGetById     = "dbo.usp_Student_GetById";
    private const string StSpEmailExists = "dbo.usp_Student_EmailExists";
    private const string StSpSave        = "dbo.usp_Student_Save";
    private const string StSpDelete      = "dbo.usp_Student_Delete";

    #endregion

    #region Fields

    private readonly AppDbContext _objDbContext;

    #endregion

    #region Constructor

    public StudentRepository(AppDbContext objDbContext)
    {
        _objDbContext = objDbContext;
    }

    #endregion

    #region Public Methods

    public async Task<(IReadOnlyList<Student> Items, int TotalCount)> GetPagedAsync(
        StudentQueryParameters objParameters,
        CancellationToken objCancellationToken = default)
    {
        SqlParameter objTotalCount = new("@TotalCount", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        IReadOnlyList<Student> lstItems = await QueryAsync(StSpGetList, MapStudent, objCancellationToken,
            Param("@PageNumber",     objParameters.PageNumber),
            Param("@PageSize",       objParameters.PageSize),
            Param("@Search",         objParameters.Search),
            Param("@Course",         objParameters.Course),
            Param("@SortBy",         objParameters.SortBy),
            Param("@SortDescending", objParameters.SortDescending),
            objTotalCount);

        int inTotal = objTotalCount.Value as int? ?? 0;
        return (lstItems, inTotal);
    }

    public async Task<Student?> GetByIdAsync(int inId, CancellationToken objCancellationToken = default)
    {
        IReadOnlyList<Student> lstItems = await QueryAsync(StSpGetById, MapStudent, objCancellationToken,
            Param("@Id", inId));

        return lstItems.FirstOrDefault();
    }

    public async Task<bool> EmailExistsAsync(
        string stEmail,
        int? inExcludeId = null,
        CancellationToken objCancellationToken = default)
    {
        object? objResult = await ScalarAsync(StSpEmailExists, objCancellationToken,
            Param("@Email",     stEmail),
            Param("@ExcludeId", inExcludeId));

        return objResult is bool isExists && isExists;
    }

    public async Task<Student> SaveAsync(Student objStudent, CancellationToken objCancellationToken = default)
    {
        IReadOnlyList<Student> lstItems = await QueryAsync(StSpSave, MapStudent, objCancellationToken,
            Param("@Id",       objStudent.Id),
            Param("@Name",     objStudent.Name),
            Param("@Email",    objStudent.Email),
            Param("@Age",      objStudent.Age),
            Param("@Course",   objStudent.Course),
            Param("@IsActive", objStudent.IsActive));

        return lstItems.First();
    }

    public async Task<bool> DeleteAsync(int inId, CancellationToken objCancellationToken = default)
    {
        object? objResult = await ScalarAsync(StSpDelete, objCancellationToken,
            Param("@Id", inId));

        int inRowsAffected = objResult as int? ?? 0;
        return inRowsAffected > 0;
    }

    #endregion

    #region Private Helpers

    private static SqlParameter Param(string stName, object? objValue)
        => new(stName, objValue ?? DBNull.Value);

    private async Task<IReadOnlyList<T>> QueryAsync<T>(
        string stStoredProcedure,
        Func<DbDataReader, T> objMap,
        CancellationToken objCancellationToken,
        params SqlParameter[] lstParameters)
    {
        await using DbCommand objCommand = await CreateCommandAsync(stStoredProcedure, lstParameters, objCancellationToken);

        List<T> lstResults = new();
        await using DbDataReader objReader = await objCommand.ExecuteReaderAsync(objCancellationToken);
        while (await objReader.ReadAsync(objCancellationToken))
        {
            lstResults.Add(objMap(objReader));
        }

        return lstResults;
    }

    private async Task<object?> ScalarAsync(
        string stStoredProcedure,
        CancellationToken objCancellationToken,
        params SqlParameter[] lstParameters)
    {
        await using DbCommand objCommand = await CreateCommandAsync(stStoredProcedure, lstParameters, objCancellationToken);
        return await objCommand.ExecuteScalarAsync(objCancellationToken);
    }

    private async Task<DbCommand> CreateCommandAsync(
        string stStoredProcedure,
        SqlParameter[] lstParameters,
        CancellationToken objCancellationToken)
    {
        DbConnection objConnection = _objDbContext.Database.GetDbConnection();
        if (objConnection.State != ConnectionState.Open)
        {
            await objConnection.OpenAsync(objCancellationToken);
        }

        DbCommand objCommand = objConnection.CreateCommand();
        objCommand.CommandText = stStoredProcedure;
        objCommand.CommandType = CommandType.StoredProcedure;
        objCommand.Parameters.AddRange(lstParameters);

        return objCommand;
    }

    private static Student MapStudent(DbDataReader objReader) => new()
    {
        Id           = (int)objReader["Id"],
        Name         = (string)objReader["Name"],
        Email        = (string)objReader["Email"],
        Age          = (int)objReader["Age"],
        Course       = (string)objReader["Course"],
        IsActive     = (bool)objReader["IsActive"],
        CreatedDate  = (DateTime)objReader["CreatedDate"],
        ModifiedDate = objReader["ModifiedDate"] as DateTime?
    };

    #endregion
}
