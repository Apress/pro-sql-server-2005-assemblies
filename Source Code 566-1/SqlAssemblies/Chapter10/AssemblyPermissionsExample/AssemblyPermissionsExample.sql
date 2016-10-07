USE AssembliesTesting;
GO

CREATE ASSEMBLY AssemblyPermissionsExample
FROM 'C:\Apress\SqlAssemblies\Chapter10\AssemblyPermissionsExample\AssemblyPermissionsExample.dll'
WITH PERMISSION_SET = EXTERNAL_ACCESS;
GO

CREATE PROCEDURE uspReadWebPage(@url nvarchar(256))
AS
EXTERNAL NAME AssemblyPermissionsExample.
[Apress.SqlAssemblies.Chapter10.AssemblyPermissionsExample].ReadWebPage;
GO

EXEC uspReadWebPage 'http://www.apress.com/';
