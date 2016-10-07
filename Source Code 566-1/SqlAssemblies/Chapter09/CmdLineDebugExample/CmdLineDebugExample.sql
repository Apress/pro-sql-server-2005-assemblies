USE AdventureWorks
GO

CREATE ASSEMBLY CmdLineDebugExample
FROM 'C:\Apress\SqlAssemblies\Chapter09\CmdLineDebugExample\CmdLineDebugExample.dll'
WITH PERMISSION_SET = SAFE;
GO

CREATE PROCEDURE uspGetContactData
AS
EXTERNAL NAME
CmdLineDebugExample.[Apress.SqlAssemblies.Chapter09.CmdLineDebugExample].GetContactData;
GO

EXEC uspGetContactData
