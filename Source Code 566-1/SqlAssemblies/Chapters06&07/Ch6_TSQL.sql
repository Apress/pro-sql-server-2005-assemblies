--Create ZipCode T-SQL UDT
CREATE TYPE [dbo].[zipcode] FROM [char](5) NULL
GO


--Create the TestUDT assembly
USE AssembliesTesting
go

CREATE ASSEMBLY TestUDT
FROM 'C:\Program Files\Microsoft SQL Server\Assemblies\TestUDT.dll'
WITH PERMISSION_SET = SAFE
GO



--Create Duration UDT
CREATE TYPE Duration
EXTERNAL NAME [TestUDT].[Duration]
GO



--Create a table to hold durations
CREATE TABLE Durations
(
  DurationId int,
  theDuration Duration NULL
)
GO 



--Select from the table
SELECT * FROM Durations
GO



--Insert some sample data
--20 seconds
INSERT Durations VALUES (1, '00:00:20') 
--15 hours, 30 minutes
INSERT Durations VALUES (2, '15:30') 
GO



--Select the data back
SELECT DurationId, theDuration.ToString() AS theDuration
FROM Durations
GO




--Duration arithmetic
DECLARE @Twenty_Hours Duration
SET @Twenty_Hours = '20:00' 
DECLARE @Forty_Seconds Duration
SET @Forty_Seconds = '00:00:40' 
SET @Twenty_Hours =
   @Twenty_Hours.AddDuration(@Forty_Seconds)
GO



--Index the duration
CREATE CLUSTERED INDEX ix_Durations ON Durations (theDuration)
GO



--Create a persisted column using a UDT property
ALTER TABLE Durations
ADD TotalHours AS (theDuration.TotalHours) PERSISTED
GO




--Query the persisted column:
SELECT *
FROM Durations
WHERE TotalHours > 3
GO




--Index the persisted column
CREATE INDEX ix_hours ON Durations (TotalHours)
GO




--Declare an instance of the EMail type
DECLARE @address EMail
SET @address = 'this_is_not_valid'
GO


--Set the address for requesting SQL Server features
DECLARE @address EMail
SET @address = 'sqlwish@microsoft.com'
GO


--Set the address for requesting SQL Server features
DECLARE @address EMail
SET @address = 'sqlwish@microsoft.com' 

--I doubt that this address exists!
SET @address.DomainAddress = 'oracle.com'
GO
