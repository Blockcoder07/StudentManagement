# Student Management - Stored Procedures

Each stored procedure used by the API lives in its own file in this folder.
Every script is **idempotent** (uses `CREATE OR ALTER`), so you can run them
any number of times without side-effects.

## Files

| # | File | Purpose |
|---|------|---------|
| 1 | `usp_Student_GetList.sql`     | Paged + searchable + sortable list. Returns rows + `@TotalCount OUTPUT`. |
| 2 | `usp_Student_GetById.sql`     | Single student by primary key. |
| 3 | `usp_Student_EmailExists.sql` | Duplicate-email check. Pass `@ExcludeId` from the Edit flow. |
| 4 | `usp_Student_Save.sql`        | UPSERT. `@Id = 0` → INSERT, `@Id > 0` → UPDATE. Returns the saved row. |
| 5 | `usp_Student_Delete.sql`      | Delete by Id. Returns `RowsAffected`. |

## Running them in SSMS

1. Open the desired `.sql` file.
2. The first line is `USE [Crud];` — the script targets the `Crud` database.
3. Press **F5** to deploy or refresh that single procedure.

## Running them all at once (PowerShell + sqlcmd)

```powershell
$server = "Vishal"
Get-ChildItem -Path . -Filter "usp_Student_*.sql" | ForEach-Object {
    & "C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE" `
        -S $server -E -i $_.FullName -b
}
```

## Verifying the deployment

```sql
SELECT  SCHEMA_NAME(schema_id) AS [Schema],
        name                   AS ProcedureName,
        create_date            AS CreatedAt,
        modify_date            AS ModifiedAt
FROM    sys.procedures
WHERE   name LIKE 'usp_Student_%'
ORDER BY name;
```
