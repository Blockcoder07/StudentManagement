using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentStoredProcedures : Migration
    {
        #region Up

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(StStudentGetList);
            migrationBuilder.Sql(StStudentGetById);
            migrationBuilder.Sql(StStudentEmailExists);
            migrationBuilder.Sql(StStudentSave);
            migrationBuilder.Sql(StStudentDelete);
        }

        #endregion

        #region Down

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF OBJECT_ID('dbo.usp_Student_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Student_Delete;");
            migrationBuilder.Sql("IF OBJECT_ID('dbo.usp_Student_Save', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Student_Save;");
            migrationBuilder.Sql("IF OBJECT_ID('dbo.usp_Student_EmailExists', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Student_EmailExists;");
            migrationBuilder.Sql("IF OBJECT_ID('dbo.usp_Student_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Student_GetById;");
            migrationBuilder.Sql("IF OBJECT_ID('dbo.usp_Student_GetList', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Student_GetList;");
        }

        #endregion

        #region Sql Constants

        private const string StStudentGetList = @"
CREATE OR ALTER PROCEDURE dbo.usp_Student_GetList
    @PageNumber     INT            = 1,
    @PageSize       INT            = 10,
    @Search         NVARCHAR(150)  = NULL,
    @Course         NVARCHAR(100)  = NULL,
    @SortBy         NVARCHAR(50)   = NULL,
    @SortDescending BIT            = 1,
    @TotalCount     INT            OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF (@PageNumber IS NULL OR @PageNumber < 1) SET @PageNumber = 1;
    IF (@PageSize   IS NULL OR @PageSize   < 1) SET @PageSize   = 10;
    IF (@PageSize   > 100)                      SET @PageSize   = 100;

    DECLARE @Skip      INT           = (@PageNumber - 1) * @PageSize;
    DECLARE @SearchLkp NVARCHAR(154) = NULLIF(LTRIM(RTRIM(@Search)), '');
    DECLARE @CourseFlt NVARCHAR(100) = NULLIF(LTRIM(RTRIM(@Course)), '');
    DECLARE @SortKey   NVARCHAR(50)  = LOWER(ISNULL(@SortBy, ''));

    IF (@SearchLkp IS NOT NULL) SET @SearchLkp = '%' + @SearchLkp + '%';

    SELECT @TotalCount = COUNT(1)
    FROM dbo.Students
    WHERE
        (@SearchLkp IS NULL OR Name  LIKE @SearchLkp
                            OR Email LIKE @SearchLkp
                            OR Course LIKE @SearchLkp)
        AND (@CourseFlt IS NULL OR Course = @CourseFlt);

    SELECT
        Id,
        Name,
        Email,
        Age,
        Course,
        IsActive,
        CreatedDate,
        ModifiedDate
    FROM dbo.Students
    WHERE
        (@SearchLkp IS NULL OR Name  LIKE @SearchLkp
                            OR Email LIKE @SearchLkp
                            OR Course LIKE @SearchLkp)
        AND (@CourseFlt IS NULL OR Course = @CourseFlt)
    ORDER BY
        CASE WHEN @SortKey = 'name'   AND @SortDescending = 0 THEN Name   END ASC,
        CASE WHEN @SortKey = 'name'   AND @SortDescending = 1 THEN Name   END DESC,
        CASE WHEN @SortKey = 'course' AND @SortDescending = 0 THEN Course END ASC,
        CASE WHEN @SortKey = 'course' AND @SortDescending = 1 THEN Course END DESC,
        CASE WHEN @SortKey = 'age'    AND @SortDescending = 0 THEN Age    END ASC,
        CASE WHEN @SortKey = 'age'    AND @SortDescending = 1 THEN Age    END DESC,
        CASE
            WHEN (@SortKey NOT IN ('name','course','age') OR @SortKey = 'createddate')
                 AND @SortDescending = 0
            THEN CreatedDate
        END ASC,
        CASE
            WHEN (@SortKey NOT IN ('name','course','age') OR @SortKey = 'createddate')
                 AND @SortDescending = 1
            THEN CreatedDate
        END DESC
    OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;
END";

        private const string StStudentGetById = @"
CREATE OR ALTER PROCEDURE dbo.usp_Student_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        Email,
        Age,
        Course,
        IsActive,
        CreatedDate,
        ModifiedDate
    FROM dbo.Students
    WHERE Id = @Id;
END";

        private const string StStudentEmailExists = @"
CREATE OR ALTER PROCEDURE dbo.usp_Student_EmailExists
    @Email     NVARCHAR(150),
    @ExcludeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CAST(
            CASE
                WHEN EXISTS (
                    SELECT 1
                    FROM dbo.Students
                    WHERE Email = @Email
                      AND (@ExcludeId IS NULL OR Id <> @ExcludeId)
                ) THEN 1
                ELSE 0
            END AS BIT
        ) AS [Exists];
END";

        private const string StStudentSave = @"
CREATE OR ALTER PROCEDURE dbo.usp_Student_Save
    @Id       INT            = 0,
    @Name     NVARCHAR(100),
    @Email    NVARCHAR(150),
    @Age      INT,
    @Course   NVARCHAR(100),
    @IsActive BIT            = 1
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF (@Id IS NULL OR @Id = 0)
    BEGIN
        INSERT INTO dbo.Students (Name, Email, Age, Course, IsActive, CreatedDate)
        VALUES (@Name, @Email, @Age, @Course, @IsActive, GETUTCDATE());

        SET @Id = CAST(SCOPE_IDENTITY() AS INT);
    END
    ELSE
    BEGIN
        UPDATE dbo.Students
        SET
            Name         = @Name,
            Email        = @Email,
            Age          = @Age,
            Course       = @Course,
            IsActive     = @IsActive,
            ModifiedDate = GETUTCDATE()
        WHERE Id = @Id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 51001, 'Student not found.', 1;
        END
    END

    SELECT
        Id,
        Name,
        Email,
        Age,
        Course,
        IsActive,
        CreatedDate,
        ModifiedDate
    FROM dbo.Students
    WHERE Id = @Id;
END";

        private const string StStudentDelete = @"
CREATE OR ALTER PROCEDURE dbo.usp_Student_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Students
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END";

        #endregion
    }
}
