/****** Object:  StoredProcedure [dbo].[SP_CategoryStoreFilter]    Script Date: 9/15/2025 12:54:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[SP_CategoryStoreFilter]
	@SearchText nvarchar(100) = null,
	@GenderId int = null,
	@Language nvarchar(100) = 'en'
AS
BEGIN
	
	select 
		(case when @Language = 'en' then  Name else NameAr end ) as Label,
		CategoryId as Value

	from 
	Categories
	where IsViewInHome = 1
	and (@SearchText is null or Name like '%' + @SearchText + '%' or NameAr like '%' + @SearchText + '%' )
	and (@GenderId is null or GenderId = @GenderId)
	order by Label
	
END
