USE AdventureWorks
GO

CREATE ASSEMBLY TableUdfExample
FROM 'C:\Apress\SqlAssemblies\Chapter05\TableUdfExample\TableUdfExample.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE FUNCTION ReadDirectoryData()
RETURNS TABLE (
   id int,
   name nvarchar(255),
   email nvarchar(255),
   [proxy addresses] nvarchar(1000),
   adspath nvarchar(1000),
   principalName nvarchar(255),
   guid uniqueidentifier)
AS EXTERNAL NAME TableUdfExample.[Apress.SqlAssemblies.Chapter05.TableUdfExample].ReadDirectoryData;
GO

SELECT id, name, [proxy addresses], adspath, guid FROM ReadDirectoryData()
WHERE guid IS NOT NULL;
