/****** Object:  StoredProcedure [dbo].[SP_SizesStoreFilter]    Script Date: 9/15/2025 2:50:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[SP_SizesStoreFilter]
	@SearchText nvarchar(100) = null,
	@GenderId int = null,
	@Language nvarchar(100) = 'en'
AS
BEGIN
	
	select 
		(case when @Language = 'en' then Name else NameAr end ) as Label,
		UnitId as Value

	from Units 
	where (@SearchText is null or Name like '%' + @SearchText + '%' or NameAr like '%' + @SearchText + '%' )
	order by Label
	
END
