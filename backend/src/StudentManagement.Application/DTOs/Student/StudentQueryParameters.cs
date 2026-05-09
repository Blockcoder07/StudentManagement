namespace StudentManagement.Application.DTOs.Student;

public class StudentQueryParameters
{
    #region Constants

    private const int InMaxPageSize = 100;

    #endregion

    #region Fields

    private int _inPageSize = 10;

    #endregion

    #region Properties

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _inPageSize;
        set => _inPageSize = value > InMaxPageSize ? InMaxPageSize : (value <= 0 ? 10 : value);
    }

    public string? Search { get; set; }

    public string? Course { get; set; }

    public string? SortBy { get; set; }

    public bool SortDescending { get; set; }

    #endregion
}
