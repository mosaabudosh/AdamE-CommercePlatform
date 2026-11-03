/****** Object:  StoredProcedure [dbo].[SP_BeautyDashboardFooter]    Script Date: 9/15/2025 5:14:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[SP_BeautyDashboardFooter]
	@Language nvarchar(100) = 'en'
AS
BEGIN
	
	select 
		(case when @Language = 'en' then Name else NameAr end ) as Label,
		CategoryId as Value

	from Categories 
	where IsViewInFooter = 1 and IsBeauty = 1
	order by Label	
END
