--Get average price of sales for the last month
SELECT AVG(Price)
FROM Sales
WHERE SalesDate >= DATEADD(mm, -1, GETDATE()) 
GO




--Create the CountNulls aggregate
CREATE AGGREGATE dbo.CountNulls(@Value VARCHAR(8000))
RETURNS INT
EXTERNAL NAME MyUDAs.CountNulls
GO




--Use the CountNulls aggregate
SELECT dbo.CountNulls(StringColumn)
FROM myTable
GO



--Create a table of durations
CREATE TABLE AvgTime
(TimeId int NOT NULL,
TimeTaken Duration null)
GO

--Insert some sample data
INSERT INTO AvgTime VALUES (1,'11:30:00')
INSERT INTO AvgTime VALUES (1,'11:32:00')
INSERT INTO AvgTime VALUES (1,'11:34:00') -- Avg 11:32:00
INSERT INTO AvgTime VALUES (2,'00:30:00')
INSERT INTO AvgTime VALUES (2,'00:32:20')
INSERT INTO AvgTime VALUES (2,'00:32:44')
INSERT INTO AvgTime VALUES (2,'00:39:21') -- Avg 2016.25 =00:33:36.25
GO 



--Create the AvgDuration aggregate
CREATE AGGREGATE AvgDuration(@TimeValue Duration)
RETURNS Duration
EXTERNAL NAME DurationObjects.AvgDuration
GO


--Use the aggregate
SELECT dbo.AvgDuration(TimeTaken) As AvgDuration
  FROM AvgTime
GROUP BY TimeId
GO




--Create the ExcelVarP aggregate
CREATE AGGREGATE ExcelVarP(@val float)
RETURNS float
EXTERNAL NAME [MyVarP].[VarP]
GO


--Create a table for test data
CREATE TABLE TestAgg
(AggId int,
AggValue bigint)
GO

--Insert some test data
INSERT INTO TestAgg VALUES (1,8)
INSERT INTO TestAgg VALUES (2,8)
INSERT INTO TestAgg VALUES (3,9)
INSERT INTO TestAgg VALUES (4,12)
INSERT INTO TestAgg VALUES (5,13)
GO

--Use the aggregate
SELECT dbo.ExcelVarP(AggValue)
FROM TestAgg
GO