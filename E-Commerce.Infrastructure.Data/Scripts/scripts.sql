
SET IDENTITY_INSERT [dbo].[Screens] ON 

INSERT [dbo].[Screens] ([ScreenId], [Name], [NameAr], [IsInMenue], [Url], [Icon], [ModuleId], [CreationUserId],[CreationDate],[IsActive]) VALUES (8, N'Cities', N'المدن',  1, N'cities', N'pi pi-map-marker', 2, N'26435193-38f2-43b2-8841-3ab288e19f9e',CAST(N'2025-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[Screens] ([ScreenId], [Name], [NameAr], [IsInMenue], [Url], [Icon], [ModuleId], [CreationUserId],[CreationDate],[IsActive]) VALUES (9, N'Countries', N'البلدان', 1, N'countries', N'pi pi-map', 2, N'26435193-38f2-43b2-8841-3ab288e19f9e',CAST(N'2025-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[Screens] ([ScreenId], [Name], [NameAr], [IsInMenue], [Url], [Icon], [ModuleId], [CreationUserId],[CreationDate],[IsActive]) VALUES (10, N'Items', N'العناصر',  1, N'items', N'pi pi-list', 2, N'26435193-38f2-43b2-8841-3ab288e19f9e',CAST(N'2025-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[Screens] ([ScreenId], [Name], [NameAr], [IsInMenue], [Url], [Icon], [ModuleId], [CreationUserId],[CreationDate],[IsActive]) VALUES (11, N'Shops', N'المحلات التجارية', 1, N'shops', N'pi pi-building', 2, N'26435193-38f2-43b2-8841-3ab288e19f9e',CAST(N'2025-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[Screens] ([ScreenId], [Name], [NameAr], [IsInMenue], [Url], [Icon], [ModuleId], [CreationUserId],[CreationDate],[IsActive]) VALUES (12, N'Taxs', N'الضرائب', 2, N'taxs', N'pi pi-asterisk', 2, N'26435193-38f2-43b2-8841-3ab288e19f9e',CAST(N'2025-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[Screens] ([ScreenId], [Name], [NameAr], [IsInMenue], [Url], [Icon], [ModuleId], [CreationUserId],[CreationDate],[IsActive]) VALUES (13, N'Categories', N'التصنيفات', 2, N'categories', N'pi pi-list', 2, N'26435193-38f2-43b2-8841-3ab288e19f9e',CAST(N'2025-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[Screens] ([ScreenId], [Name], [NameAr], [IsInMenue], [Url], [Icon], [ModuleId], [CreationUserId],[CreationDate],[IsActive]) VALUES (14, N'Carts', N'العربات', 1, N'carts', N'pi pi-shopping-cart', 2, N'26435193-38f2-43b2-8841-3ab288e19f9e',CAST(N'2025-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[Screens] ([ScreenId], [Name], [NameAr], [IsInMenue], [Url], [Icon], [ModuleId], [CreationUserId],[CreationDate],[IsActive]) VALUES (15, N'Cart Statuses', N'حالات عربة التسوق',  1, N'cart-statuses', N'pi pi-list', 2, N'26435193-38f2-43b2-8841-3ab288e19f9e',CAST(N'2025-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[Screens] ([ScreenId], [Name], [NameAr], [IsInMenue], [Url], [Icon], [ModuleId], [CreationUserId],[CreationDate],[IsActive]) VALUES (16, N'Orders', N'طلبات',  1, N'orders', N'pi pi-briefcase', 2, N'26435193-38f2-43b2-8841-3ab288e19f9e',CAST(N'2025-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[Screens] ([ScreenId], [Name], [NameAr], [IsInMenue], [Url], [Icon], [ModuleId], [CreationUserId],[CreationDate],[IsActive]) VALUES (17, N'Reports', N'التقارير', 1, N'reports', N'pi pi-file', 4, N'26435193-38f2-43b2-8841-3ab288e19f9e',CAST(N'2025-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[Screens] ([ScreenId], [Name], [NameAr], [IsInMenue], [Url], [Icon], [ModuleId], [CreationUserId],[CreationDate],[IsActive]) VALUES (18, N'Notifications', N'الإشعارات', 1, N'notifications-page', N'pi pi-bell', 3, N'26435193-38f2-43b2-8841-3ab288e19f9e',CAST(N'2025-01-01T00:00:00.0000000' AS DateTime2), 1)

SET IDENTITY_INSERT [dbo].[Screens] OFF
GO


INSERT [dbo].[Users] ([UserId], [FirstName], [SecondName], [LastName], [FirstNameAr], [SecondNameAr], [LastNameAr], [UserName], [Email], [MobileNumber], [PasswordHash], [PasswordSalt], [DateOfBirth], [ProfileImage], [Adresse], [IsBlock], [UserTypeId], [LanguageId], [GenderId], [CityId], [ShopId], [CreationUserId], [LastUpdateUserId], [CreationDate], [LastUpdatedDate], [IsActive]) VALUES (N'26435193-38f2-43b2-8841-3ab288e19f9e', N'Admin', N'Admin', N'Admin', N'أدمن', N'أدمن', N'أدمن', N'Admin', N'Admin@gmail.com', N'0788647816', 0x42BA922439232D28B34EDBE5D38AC32F4AC714A572840D6A44CC5D198FA7DDA93161401B6D0099EAF827C243076507C4DD996B97CA80263642E3E7D0E4F18926, 0x0A4D90ECFA27C9A19ED9234A0776CAC351AEDD0442A0D730398E07B19814BAAAC87CC7E1EF8E5E85E0FFF390663086B97B5544D4CF51B55A3E79BB3220310ED3A0F48D7B1F484F6A053EB5024C1D152CFE3286490C5EC0BC4878EFD72B93547A9AA2925F3274A64DAB7879BE1337F8EB5F4F72DF42CAE6813FB20E4A3A44A685, CAST(N'1970-05-06T00:00:00.0000000' AS DateTime2), NULL, N'Amman', 0, 1, 1, 1, 1, NULL, N'26435193-38f2-43b2-8841-3ab288e19f9e', N'26435193-38f2-43b2-8841-3ab288e19f9e', CAST(N'2025-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2025-05-27T05:58:08.6232977' AS DateTime2), 1)
GO


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
		    from Screens
GO

