USE AssembliesTesting
go

SET QUOTED_IDENTIFIER OFF
GO

CREATE ASSEMBLY DMLTrigger
AUTHORIZATION [dbo]
FROM "C:\Apress\SqlAssemblies\Chapter08\DMLTrigger.dll"
WITH PERMISSION_SET = EXTERNAL_ACCESS
GO

CREATE TRIGGER ColsUpd
ON AuditActions FOR INSERT
AS EXTERNAL NAME DMLTrigger.Triggers.ColsUpdDemo
GO

INSERT INTO AuditActions (TriggerName, StartTime, EndTime) 
  VALUES ("DropProcedure","06:00","20:00") 
