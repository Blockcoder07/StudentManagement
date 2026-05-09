/* ============================================================================
 * Procedure    : dbo.usp_Student_GetById
 * Author       : Student Management Team
 * Created      : 2026-05-09
 * Description  : Returns a single Student row by primary key. Returns an
 *                empty result set when the id does not exist; the C# layer
 *                then translates that into a 404 response.
 *
 * Parameters   :
 *      @Id INT     The primary key of the student to fetch.
 *
 * Returns      :
 *      Result set: Id, Name, Email, Age, Course, IsActive, CreatedDate,
 *                  ModifiedDate
 *
 * Sample call  :
 *      EXEC dbo.usp_Student_GetById @Id = 1;
 * ============================================================================ */

USE [Crud];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Student_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

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
