/* ============================================================================
 * Procedure    : dbo.usp_Student_Save        (UPSERT)
 * Author       : Student Management Team
 * Created      : 2026-05-09
 * Description  : Single procedure that handles BOTH Add and Edit operations.
 *                  - When @Id IS NULL or 0  -> INSERT a new student.
 *                  - When @Id > 0           -> UPDATE the existing student.
 *                If an UPDATE matches no row, the procedure raises
 *                THROW 51001 ("Student not found.") which the C# layer
 *                translates into a 404 response.
 *                The saved row is always returned so the API can echo it
 *                straight back to the caller.
 *
 * Parameters   :
 *      @Id       INT              0/NULL = INSERT, otherwise UPDATE.
 *      @Name     NVARCHAR(100)    Student full name.
 *      @Email    NVARCHAR(150)    Unique email (enforced by index).
 *      @Age      INT              Age (also validated by API: 5..120).
 *      @Course   NVARCHAR(100)    Course of study.
 *      @IsActive BIT              Active flag. Default 1.
 *
 * Returns      :
 *      Result set with the saved row:
 *      Id, Name, Email, Age, Course, IsActive, CreatedDate, ModifiedDate
 *
 * Sample call (INSERT):
 *      EXEC dbo.usp_Student_Save
 *           @Id       = 0,
 *           @Name     = N'Test User',
 *           @Email    = N'test.user@school.local',
 *           @Age      = 21,
 *           @Course   = N'DevOps',
 *           @IsActive = 1;
 *
 * Sample call (UPDATE):
 *      EXEC dbo.usp_Student_Save
 *           @Id       = 1,
 *           @Name     = N'Aarav Sharma',
 *           @Email    = N'aarav.sharma@example.com',
 *           @Age      = 21,
 *           @Course   = N'Computer Science',
 *           @IsActive = 1;
 * ============================================================================ */

USE [Crud];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Student_Save
    @Id       INT             = 0,
    @Name     NVARCHAR(100),
    @Email    NVARCHAR(150),
    @Age      INT,
    @Course   NVARCHAR(100),
    @IsActive BIT             = 1
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF (@Id IS NULL OR @Id = 0)
    BEGIN
        /* -- INSERT branch ------------------------------------------------- */
        INSERT INTO dbo.Students
                    (Name,  Email,  Age,  Course,  IsActive, CreatedDate)
        VALUES      (@Name, @Email, @Age, @Course, @IsActive, GETUTCDATE());

        SET @Id = CAST(SCOPE_IDENTITY() AS INT);
    END
    ELSE
    BEGIN
        /* -- UPDATE branch ------------------------------------------------- */
        UPDATE dbo.Students
        SET    Name         = @Name,
               Email        = @Email,
               Age          = @Age,
               Course       = @Course,
               IsActive     = @IsActive,
               ModifiedDate = GETUTCDATE()
        WHERE  Id = @Id;

        IF @@ROWCOUNT = 0
        BEGIN
            ;THROW 51001, N'Student not found.', 1;
        END
    END

    /* -- Echo the saved row -------------------------------------------- */
    SELECT  Id,
            Name,
            Email,
            Age,
            Course,
            IsActive,
            CreatedDate,
            ModifiedDate
    FROM    dbo.Students
    WHERE   Id = @Id;
END
GO
