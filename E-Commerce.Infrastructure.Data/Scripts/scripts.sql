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



INSERT INTO [dbo].[ScreenActions]
           ([ScreenId]
           ,[ActionId])
     select ScreenId ,1 from Screens where ScreenId not in (4,5,6,7,17,18) and screenId not in (select ScreenId from ScreenActions where ActionId = 1)
GO

INSERT INTO [dbo].[ScreenActions]
           ([ScreenId]
           ,[ActionId])
     select ScreenId ,2 from Screens where ScreenId not in (4,5,6,7,17,18) and screenId not in (select ScreenId from ScreenActions where ActionId = 2)
GO


INSERT INTO [dbo].[ScreenActions]
           ([ScreenId]
           ,[ActionId])
     select ScreenId ,3 from Screens where ScreenId not in (4,5,6,7,17,18) and screenId not in (select ScreenId from ScreenActions where ActionId = 3)
GO

INSERT INTO [dbo].[ScreenActions]
           ([ScreenId]
           ,[ActionId])
     select ScreenId ,4 from Screens where ScreenId not in (4,5,6,7,17,18) and screenId not in (select ScreenId from ScreenActions where ActionId = 4)
GO
