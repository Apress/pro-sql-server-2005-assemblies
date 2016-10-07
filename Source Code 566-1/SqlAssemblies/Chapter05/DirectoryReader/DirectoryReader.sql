USE AdventureWorks
GO

CREATE ASSEMBLY DirectoryReader
FROM 'C:\Apress\SqlAssemblies\Chapter05\DirectoryReader\DirectoryReader.dll'
WITH PERMISSION_SET = EXTERNAL_ACCESS;
GO

CREATE FUNCTION BrowseDirectory(@dirpath nvarchar(1000))
RETURNS TABLE (
   Name nvarchar(256),
   Type nvarchar(6),
   Extension nvarchar(10),
   [Time Created] datetime,
   [Time Accessed] datetime
)
AS EXTERNAL NAME DirectoryReader.[Apress.SqlAssemblies.Chapter05.DirectoryBrowserExample].BrowseDirectory;
GO

SELECT * FROM BrowseDirectory('C:\Apress\SqlAssemblies\Chapter05');
