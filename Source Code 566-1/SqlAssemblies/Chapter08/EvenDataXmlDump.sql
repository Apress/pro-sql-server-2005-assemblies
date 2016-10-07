USE AssembliesTesting
GO

CREATE TABLE EventDataDumpData(
  TriggerName varchar(50) NOT NULL,
  InsertTime  datetime    NOT NULL DEFAULT (getdate()),
  XMLData     xml         NOT NULL
);

CREATE ASSEMBLY EventDataDump
AUTHORIZATION [dbo]
FROM 'C:\Apress\SqlAssemblies\Chapter08\EventDataDump.dll'
WITH PERMISSION_SET = SAFE
GO

CREATE TRIGGER XMLDump
ON DATABASE FOR DDL_DATABASE_LEVEL_EVENTS
AS EXTERNAL NAME EventDataDump.Triggers.EventDataXMLDump
GO

SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE FirstXML
AS
BEGIN
SELECT "Just A Test"
END
GO
SELECT * FROM EventDataDumpData
GO

