USE AdventureWorks;
GO

CREATE ASSEMBLY EventLogExample
FROM 'C:\Apress\SqlAssemblies\Chapter09\EventLogExample\EventLogExample.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE PROCEDURE uspGetDataForNamedEmployee(@name nvarchar(512))
AS
EXTERNAL NAME EventLogExample.[Apress.SqlAssemblies.Chapter09.EventLogExample].GetDataForNamedEmployee;
GO

EXEC uspGetDataForNamedEmployee 'Carla Adams';