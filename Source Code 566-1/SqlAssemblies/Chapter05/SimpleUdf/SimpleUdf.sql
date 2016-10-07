USE AdventureWorks
GO

sp_configure 'clr enabled', 1;
GO
RECONFIGURE;
GO

ALTER DATABASE AdventureWorks
SET TRUSTWORTHY ON;

CREATE TABLE employees
(
   id int IDENTITY PRIMARY KEY,
   name nvarchar(255),
   email nvarchar(255)
);
GO

INSERT INTO employees
VALUES ('Julian Skinner', 'Julian@JulianSkinner.com');
INSERT INTO employees
VALUES ('Administrator', 'Administrator@JulianSkinner.com');
GO

CREATE ASSEMBLY [System.DirectoryServices]
FROM 'C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.DirectoryServices.dll'
WITH PERMISSION_SET = UNSAFE;

CREATE ASSEMBLY SimpleUdf
FROM 'C:\Apress\SqlAssemblies\Chapter05\SimpleUdf\SimpleUdf.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE FUNCTION GetProxyAddresses(@email nvarchar(255))
RETURNS nvarchar(1000)
AS
EXTERNAL NAME
     SimpleUdf.[Apress.SqlAssemblies.Chapter05.SimpleUdf].GetProxyAddresses;
GO

SELECT id, name, email, dbo.GetProxyAddresses(email) AS [Proxy Addresses]
FROM employees;
