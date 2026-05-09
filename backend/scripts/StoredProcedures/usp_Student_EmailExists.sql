/* ============================================================================
 * Procedure    : dbo.usp_Student_EmailExists
 * Author       : Student Management Team
 * Created      : 2026-05-09
 * Description  : Checks whether an email is already used by another student.
 *                Pass @ExcludeId in the Edit flow to ignore the row being
 *                edited (otherwise the row would always look "duplicate").
 *
 * Parameters   :
 *      @Email     NVARCHAR(150)    The email address to check.
 *      @ExcludeId INT              Optional. If set, the row with this Id is
 *                                  ignored from the duplicate check.
 *
 * Returns      :
 *      Single row, single column [Exists] BIT.
 *      1 = a duplicate exists, 0 = email is free.
 *
 * Sample call  :
 *      EXEC dbo.usp_Student_EmailExists
 *           @Email     = N'aarav.sharma@example.com',
 *           @ExcludeId = NULL;
 * ============================================================================ */

USE [Crud];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Student_EmailExists
    @Email     NVARCHAR(150),
    @ExcludeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
               CASE
                   WHEN EXISTS (
                       SELECT 1
                       FROM   dbo.Students
                       WHERE  Email = @Email
                         AND  (@ExcludeId IS NULL OR Id <> @ExcludeId)
                   ) THEN 1
                   ELSE 0
               END AS BIT
           ) AS [Exists];
END
GO
