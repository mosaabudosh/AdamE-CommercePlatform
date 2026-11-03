/****** Object:  StoredProcedure [dbo].[SP_ColorsStoreFilter]    Script Date: 9/15/2025 2:45:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[SP_ColorsStoreFilter]
	@SearchText nvarchar(100) = null,
	@GenderId int = null,
	@Language nvarchar(100) = 'en'
AS
BEGIN
	
	select 
		(case when @Language = 'en' then Name else NameAr end ) as Label,
		ColorId as Value

	from Colors 
	where (@SearchText is null or Name like '%' + @SearchText + '%' or NameAr like '%' + @SearchText + '%' )
	order by Label
	
END
