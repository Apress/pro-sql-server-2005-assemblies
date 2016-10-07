USE AdventureWorks
GO

CREATE ASSEMBLY EmailExample
FROM 'C:\Apress\SqlAssemblies\Chapter09\EmailExample\EmailExample.dll'
WITH PERMISSION_SET = EXTERNAL_ACCESS;
GO

CREATE PROCEDURE uspGetEmployeeData(@columnList nvarchar(1000), @id int)
AS
EXTERNAL NAME EmailExample.[Apress.SqlAssemblies.Chapter09.EmailExample].GetEmployeeData;
GO

EXEC uspGetEmployeeData 'NationalIDNumber', 1;
