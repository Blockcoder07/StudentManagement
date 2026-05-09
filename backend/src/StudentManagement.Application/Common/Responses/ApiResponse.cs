namespace StudentManagement.Application.Common.Responses;

public class ApiResponse<T>
{
    #region Properties

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public IEnumerable<string>? Errors { get; set; }

    public int StatusCode { get; set; }

    #endregion

    #region Factory Methods

    public static ApiResponse<T> Ok(T objData, string stMessage, int inStatusCode = 200) => new()
    {
        Success = true,
        Message = stMessage,
        Data = objData,
        StatusCode = inStatusCode
    };

    public static ApiResponse<T> Fail(string stMessage, int inStatusCode, IEnumerable<string>? lstErrors = null) => new()
    {
        Success = false,
        Message = stMessage,
        Errors = lstErrors,
        StatusCode = inStatusCode
    };

    #endregion
}

public class ApiResponse : ApiResponse<object>
{
    #region Factory Methods

    public static ApiResponse Ok(string stMessage, int inStatusCode = 200) => new()
    {
        Success = true,
        Message = stMessage,
        StatusCode = inStatusCode
    };

    public new static ApiResponse Fail(string stMessage, int inStatusCode, IEnumerable<string>? lstErrors = null) => new()
    {
        Success = false,
        Message = stMessage,
        Errors = lstErrors,
        StatusCode = inStatusCode
    };

    #endregion
}
