/* ============================================================================
 * Procedure    : dbo.usp_Student_GetList
 * Author       : Student Management Team
 * Created      : 2026-05-09
 * Description  : Returns a paged, searchable, sortable slice of dbo.Students.
 *                The total filtered row count is returned via @TotalCount
 *                OUTPUT so the API can build pagination metadata in a single
 *                round-trip.
 *
 * Parameters   :
 *      @PageNumber     INT             1-based page index. Default 1.
 *      @PageSize       INT             Rows per page (1-100). Default 10.
 *      @Search         NVARCHAR(150)   Optional substring (Name/Email/Course).
 *      @Course         NVARCHAR(100)   Optional exact-match Course filter.
 *      @SortBy         NVARCHAR(50)    name | course | age | createddate.
 *                                      Anything else falls back to CreatedDate.
 *      @SortDescending BIT             1 = DESC (default), 0 = ASC.
 *      @TotalCount     INT  OUTPUT     Total rows matching the filter.
 *
 * Returns      :
 *      Result set: Id, Name, Email, Age, Course, IsActive, CreatedDate,
 *                  ModifiedDate
 *
 * Sample call  :
 *      DECLARE @Total INT;
 *      EXEC dbo.usp_Student_GetList
 *           @PageNumber     = 1,
 *           @PageSize       = 5,
 *           @Search         = NULL,
 *           @Course         = NULL,
 *           @SortBy         = 'name',
 *           @SortDescending = 0,
 *           @TotalCount     = @Total OUTPUT;
 *      SELECT @Total AS TotalCount;
 * ============================================================================ */

USE [Crud];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Student_GetList
    @PageNumber     INT             = 1,
    @PageSize       INT             = 10,
    @Search         NVARCHAR(150)   = NULL,
    @Course         NVARCHAR(100)   = NULL,
    @SortBy         NVARCHAR(50)    = NULL,
    @SortDescending BIT             = 1,
    @TotalCount     INT             OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    /* -- Defensive defaults ------------------------------------------------ */
    IF (@PageNumber IS NULL OR @PageNumber < 1)  SET @PageNumber = 1;
    IF (@PageSize   IS NULL OR @PageSize   < 1)  SET @PageSize   = 10;
    IF (@PageSize   > 100)                       SET @PageSize   = 100;

    DECLARE @Skip      INT             = (@PageNumber - 1) * @PageSize;
    DECLARE @SearchLkp NVARCHAR(154)   = NULLIF(LTRIM(RTRIM(@Search)), N'');
    DECLARE @CourseFlt NVARCHAR(100)   = NULLIF(LTRIM(RTRIM(@Course)), N'');
    DECLARE @SortKey   NVARCHAR(50)    = LOWER(ISNULL(@SortBy, N''));

    IF (@SearchLkp IS NOT NULL)
        SET @SearchLkp = N'%' + @SearchLkp + N'%';

    /* -- Total count ------------------------------------------------------- */
    SELECT @TotalCount = COUNT(1)
    FROM   dbo.Students
    WHERE  (@SearchLkp IS NULL
            OR Name   LIKE @SearchLkp
            OR Email  LIKE @SearchLkp
            OR Course LIKE @SearchLkp)
      AND  (@CourseFlt IS NULL OR Course = @CourseFlt);

    /* -- Page payload ------------------------------------------------------ */
    SELECT  Id,
            Name,
            Email,
            Age,
            Course,
            IsActive,
            CreatedDate,
            ModifiedDate
    FROM    dbo.Students
    WHERE   (@SearchLkp IS NULL
             OR Name   LIKE @SearchLkp
             OR Email  LIKE @SearchLkp
             OR Course LIKE @SearchLkp)
      AND   (@CourseFlt IS NULL OR Course = @CourseFlt)
    ORDER BY
            CASE WHEN @SortKey = N'name'   AND @SortDescending = 0 THEN Name   END ASC,
            CASE WHEN @SortKey = N'name'   AND @SortDescending = 1 THEN Name   END DESC,
            CASE WHEN @SortKey = N'course' AND @SortDescending = 0 THEN Course END ASC,
            CASE WHEN @SortKey = N'course' AND @SortDescending = 1 THEN Course END DESC,
            CASE WHEN @SortKey = N'age'    AND @SortDescending = 0 THEN Age    END ASC,
            CASE WHEN @SortKey = N'age'    AND @SortDescending = 1 THEN Age    END DESC,
            CASE WHEN (@SortKey NOT IN (N'name', N'course', N'age') OR @SortKey = N'createddate')
                  AND @SortDescending = 0
                 THEN CreatedDate END ASC,
            CASE WHEN (@SortKey NOT IN (N'name', N'course', N'age') OR @SortKey = N'createddate')
                  AND @SortDescending = 1
                 THEN CreatedDate END DESC
    OFFSET  @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
