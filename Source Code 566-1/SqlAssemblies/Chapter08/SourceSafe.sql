USE AssembliesTesting
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE TABLE VSS (
  VSS_Ident bigint NOT NULL IDENTITY (1,  1),
  ObjectName varchar(100)  NOT NULL,
  ObjectType varchar(20)  NOT NULL,
  LoginName varchar(80)  NOT NULL,
  DatabaseName varchar(30)  NOT NULL,
  PostTime datetime NOT NULL,
  ObjectDetails text)
GO

INSERT INTO AuditActions (TriggerName,  StartTime,  EndTime) 
  VALUES ("CreateTable","07:00","18:00")
INSERT INTO AuditActions (TriggerName,  StartTime,  EndTime) 
  VALUES ("AlterTable","07:00","18:00")
GO

CREATE ASSEMBLY trgSourceSafe
AUTHORIZATION [dbo]
FROM "C:\Apress\SqlAssemblies\Chapter08\trgSourceSafe.dll"
WITH PERMISSION_SET = SAFE
go

CREATE TRIGGER trgVSS
ON DATABASE FOR DDL_DATABASE_LEVEL_EVENTS
AS EXTERNAL NAME trgSourceSafe.[Apress.SqlAssemblies.Chapter08.Triggers].clsSS
GO


