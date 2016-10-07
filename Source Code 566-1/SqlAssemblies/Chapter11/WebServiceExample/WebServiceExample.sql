USE AdventureWorks;
GO

IF EXISTS(SELECT object_id FROM sys.tables WHERE name='Person.CityDetails')
   DROP TABLE Person.CityDetails
GO


CREATE TABLE Person.CityDetails
(
   CityDetailsID int IDENTITY PRIMARY KEY,
   AddressID     int FOREIGN KEY REFERENCES Person.Address(AddressID),
   Name          nvarchar(256),
   Latitude      float,
   Longitude     float,
   Population    int,
   Image         image,
   CONSTRAINT UNQ_NameLatLong UNIQUE (Name, Latitude, Longitude)
);
GO

IF EXISTS(SELECT object_id FROM sys.procedures WHERE name='uspGetCityData')
   DROP PROCEDURE uspGetCityData

IF EXISTS(SELECT assembly_id FROM sys.assemblies WHERE name='WebServiceExample.XmlSerializers')
   DROP ASSEMBLY [WebServiceExample.XmlSerializers]

IF EXISTS(SELECT assembly_id FROM sys.assemblies WHERE name='WebServiceExample')
   DROP ASSEMBLY WebServiceExample
GO


CREATE ASSEMBLY WebServiceExample
FROM 'C:\Apress\SqlAssemblies\Chapter11\WebServiceExample\WebServiceExample.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE ASSEMBLY [WebServiceExample.XmlSerializers]
FROM 'C:\Apress\SqlAssemblies\Chapter11\WebServiceExample\WebServiceExample.XmlSerializers.dll'
WITH PERMISSION_SET = SAFE;
GO

CREATE PROCEDURE uspGetCityData(@addressID int)
AS
EXTERNAL NAME WebServiceExample.[Apress.SqlAssemblies.Chapter11.WebServiceExample].GetCityData
GO



EXEC uspGetCityData 100

SELECT * FROM Person.CityDetails
