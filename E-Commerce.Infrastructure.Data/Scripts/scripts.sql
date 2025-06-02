INSERT INTO [dbo].[RoleScreens]
           ([RoleId]
           ,[ScreenId]
           ,[CreationUserId]
           ,[CreationDate]
           ,[IsActive])
     select
           1
           ,ScreenId
           ,'26435193-38F2-43B2-8841-3AB288E19F9E'
           ,'2025-01-01 00:00:00.0000000'
           ,1
		    from Screens where ScreenId not in (select ScreenId from RoleScreens)
GO


/**********************************************************************************************************************************/


DECLARE @ActionId int = 0
WHILE @ActionId < 4
BEGIN
	   SET @ActionId = @ActionId + 1
	   INSERT INTO [dbo].[ScreenActions] ([ScreenId],[ActionId])
	   select ScreenId , @ActionId
	   from Screens 
	   where ScreenId not in (4,5,6,7,17,18) and screenId not in (select ScreenId from ScreenActions where ActionId = @ActionId)
END


/**********************************************************************************************************************************/


DECLARE @RoleScreens TABLE(RoleScreenId INT)
INSERT INTO @RoleScreens select RoleScreenId from RoleScreens where ScreenId not in (4,5,6,7,17,18)
--SELECT * FROM @RoleScreens

DECLARE @ActionId int = 0
WHILE @ActionId < 4
BEGIN

	SET @ActionId = @ActionId + 1
	INSERT INTO [dbo].[RoleScreenActions] ([RoleScreenId],[ActionId],[CreationUserId],[CreationDate],[IsActive])
    select RoleScreenId , @ActionId ,'26435193-38F2-43B2-8841-3AB288E19F9E' , GetDate() ,1
	from @RoleScreens where RoleScreenId not in (select RoleScreenId from RoleScreenActions where RoleScreenId = RoleScreenId and ActionId = @ActionId)

END

--select * from RoleScreenActions


/**********************************************************************************************************************************/
