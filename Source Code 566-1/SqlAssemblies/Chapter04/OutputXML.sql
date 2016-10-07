USE AdventureWorks
GO

CREATE PROCEDURE SpecialOffers @OfferID int = NULL
AS
BEGIN
SELECT so.Description, p.Name, so.DiscountPct, so.MinQty,
       ISNULL(CONVERT(varchar(10),so.MaxQty),'No max') AS MaxQty
   FROM Sales.SpecialOfferProduct sop
   JOIN Sales.SpecialOffer so ON so.SpecialOfferID = sop.SpecialOfferID
   JOIN Production.Product p ON p.ProductID = sop.ProductID
   WHERE sop.SpecialOfferID >= ISNULL(@OfferID,0)
   AND sop.SpecialOfferID <= ISNULL(@OfferID,999)
END
GO

SET QUOTED_IDENTIFIER OFF
GO
CREATE ASSEMBLY XMLOutput
AUTHORIZATION [dbo]
FROM "C:\Program Files\SQL Server Assemblies\XMLOutput.dll"
WITH PERMISSION_SET = EXTERNAL_ACCESS
GO

CREATE PROCEDURE XMLDoc (@SprocName nvarchar(255), @RootName nvarchar(255),
@FileName nvarchar(255))
AS EXTERNAL NAME XMLOutput.[Apress.XML.StoredProcedures].OutputXML
GO

CREATE PROCEDURE RSSDoc (@SprocName nvarchar(255), @FileName nvarchar(255), @Title nvarchar(255), @Link nvarchar(255), @Description nvarchar(255))
AS EXTERNAL NAME XMLOutput.[Apress.XML.StoredProcedures].OutputXML
GO

XMLDoc 'SpecialOffers', 'ROOT', 'C:\Discounts.XML' 
XMLDoc 'SpecialOffers', 'C:\DiscountsRSS.XML', 'AdventureWorks Special Offers', 'http://www.AdventureWorks.com/specialoffers/', 'Special offers on Mountain Bike gear from AdventureWorks'
GO
