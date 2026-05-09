/* ============================================================================
 * Procedure    : dbo.usp_Student_Delete
 * Author       : Student Management Team
 * Created      : 2026-05-09
 * Description  : Deletes a student row by primary key. Returns the affected
 *                row count so the C# layer can convert 0 into a 404.
 *
 * Parameters   :
 *      @Id INT     Primary key of the student to delete.
 *
 * Returns      :
 *      Single row, single column [RowsAffected] INT.
 *      0 = no matching id, 1 = success.
 *
 * Sample call  :
 *      EXEC dbo.usp_Student_Delete @Id = 99;
 * ============================================================================ */

USE [Crud];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Student_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Students
    WHERE  Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
