USE AssembliesTesting
GO

CREATE ASSEMBLY OSCmdExample
FROM 'C:\Apress\SqlAssemblies\Chapter04\OSCmdExample.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE PROCEDURE uspExecuteOSCmd(@filename nvarchar(256))
AS
EXTERNAL NAME OSCmdExample.[Apress.SqlAssemblies.Chapter04.OSCmdExample].ExecuteOSCmd;
GO

EXEC uspExecuteOSCmd 'tasklist.exe';