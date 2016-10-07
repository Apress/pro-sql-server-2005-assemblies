USE AssembliesTesting
GO

CREATE TABLE AuditActions(
AuditId BIGINT IDENTITY(1,1), 
TriggerName VARCHAR(100),
StartTime CHAR(5),
EndTime CHAR(5))
GO
SET QUOTED_IDENTIFIER OFF
GO

INSERT INTO AuditActions (TriggerName,  StartTime,  EndTime) 
  VALUES ("AlterProcedure","07:00","18:00")
INSERT INTO AuditActions (TriggerName,  StartTime,  EndTime) 
  VALUES ("CreateProcedure","07:00","18:00")
GO

CREATE ASSEMBLY TriggerAudit
AUTHORIZATION [dbo]
FROM "C:\Apress\SqlAssemblies\Chapter08\triggeraudit.dll"
WITH PERMISSION_SET = SAFE
GO

CREATE TRIGGER TriggerAuditing
ON DATABASE FOR  DDL_DATABASE_LEVEL_EVENTS
AS EXTERNAL NAME TriggerAudit.Triggers.AuditTrigger
GO

CREATE PROCEDURE TrigAudTest
AS
BEGIN
SELECT "TriggerAudit Test"
END


