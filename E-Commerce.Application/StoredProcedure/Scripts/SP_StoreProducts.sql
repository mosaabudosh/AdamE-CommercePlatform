USE [Adam-E-Commerce-DB]
GO
/****** Object:  StoredProcedure [dbo].[SP_StoreProducts]    Script Date: 7/2/2025 10:49:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


Create or ALTER PROCEDURE [dbo].[SP_StoreProducts]
	@PageNumber int = null,
	@PageSize int = null,
	@SearchText nvarchar(100) = null,
	@CategoryIds nvarchar(50) = null

AS
BEGIN
	set @PageSize = ISNULL(@PageSize, 2147483647)
	set @PageNumber = ISNULL(@PageNumber, 1)

	declare @CategoriesIds table (CategoryId nvarchar(20))
	if(@CategoryIds <> '' or @CategoryIds is not null)
	begin
		insert into @CategoriesIds
		select * from string_split(@CategoryIds, ';')
	end

	select 
		si.ShopItemId,
		i.Name,
		i.NameAr,
		i.Code,
		ISNULL(i.Descreption, ' - ') as Descreption ,
		si.Price,
		c.Name as Category,
		si.InStocks as Quantity,
		3 as Rating,
		0 as IsInFavorite,
		s.Name as Shope,
		s.ShopId,
		0 as Discount,
		t.Value as Tax,
		0 as SelectedUnitId

	from ShopItems as si left outer join
	Items as i on si.ItemId = i.ItemId left outer join
	Categories as c on i.CategoryId = c.CategoryId left outer join
	Shops as s on si.ShopId = s.ShopId left outer join
	Taxs as t on si.TaxId = t.TaxId
	where si.ShopItemStatusId = 1 
	and (
			@SearchText is null 
			or @SearchText = '' 
			or i.Name like '%@' + @SearchText + '%' 
			or i.Code like '%@' + @SearchText + '%' 
			or c.Name like '%@' + @SearchText + '%'
			or s.Name like '%@' + @SearchText + '%'
		)
	and (
			@CategoryIds is null 
			or @CategoryIds = '' 
			or c.CategoryId in (select CategoryId from @CategoriesIds)
		)

	order by Category
	OFFSET (@PageNumber-1) * @PageSize rows
	fetch next @PageSize rows ONLY;
END
