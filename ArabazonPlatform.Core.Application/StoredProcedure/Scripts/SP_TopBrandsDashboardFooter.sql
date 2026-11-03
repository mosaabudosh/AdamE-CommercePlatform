/****** Object:  StoredProcedure [dbo].[SP_TopBrandsDashboardFooter]    Script Date: 9/15/2025 5:04:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[SP_TopBrandsDashboardFooter]
	@Language nvarchar(100) = 'en'
AS
BEGIN
	
	select 
		(case when @Language = 'en' then Name else NameAr end ) as Label,
		BrandId as Value

	from Brands 
	where IsViewInFooter = 1
	order by Label	
END
