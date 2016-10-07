USE AssembliesTesting
GO

CREATE PROCEDURE uspGetSystemStats
(
   @connections int OUT,
   @cpuBusy     int OUT,
   @idle        int OUT,
   @ioBusy      int OUT,
   @packErrors  int OUT,
   @packRecd    int OUT,
   @packSent    int OUT,
   @timeticks   int OUT,
   @totErrors   int OUT,
   @totRead     int OUT,
   @totWrite    int OUT
)
AS
BEGIN
   SET @connections = @@CONNECTIONS;
   SET @cpuBusy = @@CPU_BUSY;
   SET @idle = @@IDLE;
   SET @ioBusy = @@IO_BUSY;
   SET @packErrors = @@PACKET_ERRORS;
   SET @packRecd = @@PACK_RECEIVED;
   SET @packSent = @@PACK_SENT;
   SET @timeticks = @@TIMETICKS;
   SET @totErrors = @@TOTAL_ERRORS;
   SET @totRead = @@TOTAL_READ;
   SET @totWrite = @@TOTAL_WRITE;
END;
GO

CREATE TABLE SystemStats
(
   ID               int IDENTITY PRIMARY KEY,
   time             DateTime,
   Connections      int,
   TotalConnections int,
   CpuBusy          bigint,
   TotalCpuBusy     bigint,
   Idle             bigint,
   TotalIdle        bigint,
   IoBusy           bigint,
   TotalIoBusy      bigint,
   PackErrors       int,
   TotalPackErrors  int,
   PackRecd         int,
   TotalPackRecd    int,
   PackSent         int,
   TotalPackSent    int,
   NumErrors        int,
   TotalErrors      int,
   NumReads         int,
   TotalRead        int,
   NumWrites        int,
   TotalWrite       int
);
GO

CREATE XML SCHEMA COLLECTION
   [http://schemas.apress.com/sqlassemblies/StatsRequestSchema]
AS N'<?xml version="1.0"?>
<xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
   <xs:element name="sysStatsMessage">
      <xs:complexType>
         <xs:sequence minOccurs="1" maxOccurs="1">
            <xs:element name="connections" type="xs:integer" />
            <xs:element name="cpuBusy" type="xs:integer" />
            <xs:element name="idle" type="xs:integer" />
            <xs:element name="ioBusy" type="xs:integer" />
            <xs:element name="packErrors" type="xs:integer" />
            <xs:element name="packRecd" type="xs:integer" />
            <xs:element name="packSent" type="xs:integer" />
            <xs:element name="totalErrors" type="xs:integer" />
            <xs:element name="totalRead" type="xs:integer" />
            <xs:element name="totalWrite" type="xs:integer" />
            <xs:element name="time" type="xs:dateTime" />
         </xs:sequence>
      </xs:complexType>
   </xs:element>
</xs:schema>';

CREATE MESSAGE TYPE [http://schemas.apress.com/sqlassemblies/StatsRequestMessage]
VALIDATION = VALID_XML WITH SCHEMA COLLECTION [http://schemas.apress.com/sqlassemblies/StatsRequestSchema];

CREATE CONTRACT [http://schemas.apress.com/sqlassemblies/StatsServiceContract]
(
   [http://schemas.apress.com/sqlassemblies/StatsRequestMessage] SENT BY INITIATOR
);

CREATE QUEUE StatsServiceQueue
WITH STATUS = ON, RETENTION = OFF;

CREATE SERVICE [http://schemas.apress.com/sqlassemblies/StatsRequestService]
ON QUEUE StatsServiceQueue
(
   [http://schemas.apress.com/sqlassemblies/StatsServiceContract]
);

CREATE SERVICE [http://schemas.apress.com/sqlassemblies/StatsProcessorService]
ON QUEUE StatsServiceQueue
(
   [http://schemas.apress.com/sqlassemblies/StatsServiceContract]
);
GO

CREATE ASSEMBLY ServiceBrokerClient
FROM 'C:\Apress\SqlAssemblies\Chapter11\ServiceBrokerExample\ServiceBrokerClient.dll'
WITH PERMISSION_SET = SAFE;
GO

CREATE PROCEDURE uspWriteStatsToQueue
AS
EXTERNAL NAME ServiceBrokerClient.[Apress.SqlAssemblies.Chapter11.ServiceBrokerClient].WriteStatsToQueue;
GO

CREATE ASSEMBLY ServiceBrokerServer
FROM 'C:\Apress\SqlAssemblies\Chapter11\ServiceBrokerExample\ServiceBrokerServer.dll'
WITH PERMISSION_SET = SAFE;
GO

CREATE PROCEDURE uspProcessMessages
AS
EXTERNAL NAME ServiceBrokerServer.[Apress.SqlAssemblies.Chapter11.StatsProcessorService].ProcessMessages;
GO
