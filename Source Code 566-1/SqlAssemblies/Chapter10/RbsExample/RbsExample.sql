USE AssembliesTesting
GO

ALTER DATABASE AssembliesTesting
SET TRUSTWORTHY ON;
GO

CREATE ASSEMBLY MemoReader
FROM 'C:\Apress\SqlAssemblies\Chapter10\RbsExample\MemoReader.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE ASSEMBLY RbsExample
FROM 'C:\Apress\SqlAssemblies\Chapter10\RbsExample\RbsExample.dll'
WITH PERMISSION_SET = EXTERNAL_ACCESS;
GO

CREATE FUNCTION ReadMemos(@directoryName nvarchar(256))
RETURNS TABLE
(
   Sender    nvarchar(256),
   Recipient nvarchar(256),
   Subject   nvarchar(256),
   Body      nvarchar(4000)
)
AS
EXTERNAL NAME RbsExample.[Apress.SqlAssemblies.Chapter10.RbsExample].ReadMemos;
GO

SELECT * FROM ReadMemos('C:\Apress\SqlAssemblies\Chapter10\RbsExample');