/****** Object:  StoredProcedure [dbo].[SP_ShopStoreFilter]    Script Date: 9/15/2025 12:54:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[SP_ShopStoreFilter]
	@SearchText nvarchar(100) = null,
	@GenderId int = null,
	@Language nvarchar(100) = 'en'
AS
BEGIN
	
	select 
		CommercialName as Label,
		ShopId as Value

	from 
	Shops
	where IsViewInHome = 1
	and (@SearchText is null or Name like '%' + @SearchText + '%' or CommercialName like '%' + @SearchText + '%' )
	order by Label
	
END
