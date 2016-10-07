USE AssembliesTesting
GO

CREATE TABLE StockPrices
(
   id        int identity primary key,
   symbol    nchar(4),
   price     float not null,
   tradetime datetime not null,
   change    float not null,
   openprice float not null,
   volume    int not null
);
GO

CREATE ASSEMBLY CasExample
FROM 'C:\Apress\SqlAssemblies\Chapter10\CasExample\CasExample.dll'
WITH PERMISSION_SET = EXTERNAL_ACCESS;
GO

CREATE PROCEDURE uspInsertLatestStockPrice(@symbol nchar(4))
AS
EXTERNAL NAME CasExample.[Apress.SqlAssemblies.Chapter10.CasExample].InsertLatestStockPrice;
GO

EXEC uspInsertLatestStockPrice 'MSFT';
SELECT * FROM StockPrices;
GO
