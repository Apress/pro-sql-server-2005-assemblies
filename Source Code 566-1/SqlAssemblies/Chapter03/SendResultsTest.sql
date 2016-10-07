USE AdventureWorks
GO

CREATE ASSEMBLY SendResultsTest
FROM 'C:\Apress\SqlAssemblies\Chapter03\SendResultsTest.dll'
WITH PERMISSION_SET = SAFE;
GO

CREATE PROCEDURE uspGetPayRiseDates
AS
EXTERNAL NAME SendResultsTest.[Apress.SqlAssemblies.Chapter03.SendResultsTest].GetPayRiseDates;
GO

EXEC uspGetPayRiseDates