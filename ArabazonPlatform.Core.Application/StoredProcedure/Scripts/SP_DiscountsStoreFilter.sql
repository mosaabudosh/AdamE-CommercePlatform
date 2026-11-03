/****** Object:  StoredProcedure [dbo].[SP_DiscountsStoreFilter]    Script Date: 9/15/2025 4:18:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[SP_DiscountsStoreFilter]
	@SearchText nvarchar(100) = null,
	@GenderId int = null,
	@Language nvarchar(100) = 'en'
AS
BEGIN
	
	select 
		(case when @Language = 'en' then Name else NameAr end ) as Label,
		DiscountId as Value

	from Discounts 
	where (@SearchText is null or Name like '%' + @SearchText + '%' or NameAr like '%' + @SearchText + '%' )
	and IsDeleted = 0
	order by Label
	
END
