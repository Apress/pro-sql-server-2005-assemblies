USE master;
GO

sp_configure 'clr enabled', 1;
GO
RECONFIGURE;
GO

USE AdventureWorks;
GO

CREATE ASSEMBLY SimpleSprocTest
FROM 'C:\Apress\SqlAssemblies\Chapter02\SimpleSprocTest.dll'
WITH PERMISSION_SET = SAFE;
GO

CREATE PROCEDURE uspGetEmployeesHiredAfter(@hireDate datetime)
AS
EXTERNAL NAME SimpleSprocTest.[Apress.SqlAssemblies.Chapter02.SimpleSprocTest].GetEmployeesHiredAfter;
GO

EXEC uspGetEmployeesHiredAfter '01/01/2002';