USE AssembliesTesting
GO

CREATE TABLE ImageLoadTest (
ImageId int NOT NULL IDENTITY(1,1 ),
ImageName nvarchar(255)  NOT NULL,
LoadedImage image NOT NULL)
GO

CREATE PROCEDURE InsertImage @ImageName as nvarchar(255),  @ImageToLoad image
AS
BEGIN
  INSERT INTO ImageLoadTest (ImageName,  LoadedImage)
  VALUES(@ImageName,  @ImageToLoad)
END
GO

CREATE PROCEDURE RetrieveImage @Id int
AS
BEGIN
  SELECT ImageName,LoadedImage
  FROM ImageLoadTest
  WHERE ImageId = @Id
END
GO

SET QUOTED_IDENTIFIER OFF
GO
USE AssembliesTesting


DROP ASSEMBLY LoadUnloadImages
GO

CREATE ASSEMBLY LoadUnloadImages
AUTHORIZATION [dbo]
FROM "C:\Apress\SqlAssemblies\Chapter04\ImageManipulation.dll"
WITH PERMISSION_SET = EXTERNAL_ACCESS
GO


CREATE PROCEDURE LoadASingleImage (@Loc nvarchar(255))
AS EXTERNAL NAME LoadUnloadImages.[FatBelly.Images.StoredProcedures].LoadAnyImage
GO

CREATE PROCEDURE LoadingAllImages (@Loc nvarchar(255),  @Type nvarchar(255))
AS EXTERNAL NAME LoadUnloadImages.[FatBelly.Images.StoredProcedures].LoadAllImages
GO

CREATE PROCEDURE ExtractAnImage (@ImageId int,  @Loc nvarchar(255))
AS EXTERNAL NAME LoadUnloadImages.[FatBelly.Images.StoredProcedures].ExtractImage
GO

LoadASingleImage "C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\winter.jpg"
GO

LoadingAllImages "C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures","jpg"
go

SELECT * FROM ImageLoadTest
go

ExtractAnImage 1," c:\temp"
GO
ExtractAnImage 2," c:\temp"
GO
ExtractAnImage 10," c:\temp"
GO
