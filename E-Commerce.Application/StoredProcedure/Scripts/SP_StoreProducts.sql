USE [Adam-E-Commerce-DB]
GO
/****** Object:  StoredProcedure [dbo].[SP_StoreProducts]    Script Date: 7/8/2025 10:48:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


Create or ALTER PROCEDURE [dbo].[SP_StoreProducts]
	@PageNumber int = null,
	@PageSize int = null,
	@SearchText nvarchar(100) = null,
	@CategoryIds nvarchar(100) = null,
	@ShopIds nvarchar(100) = null,
	@IsLoadImage bit = null

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

	declare @ShopeIds table (ShopId nvarchar(20))
	if(@ShopIds <> '' or @ShopIds is not null)
	begin
		insert into @ShopeIds
		select * from string_split(@ShopIds, ';')
	end

	;with ShopItemData as (
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
		null as SelectedUnitId,
		(
			case when @IsLoadImage is not null and @IsLoadImage = 1 
			then (select STRING_AGG(Path,';') from Files where FolderId = fo.FolderId) 
			else null 
			end
		) as ImagesPaths,
		u.UnitId as UnitId,
		u.Name as UnitName,
		si.UnitGroupId

	from ShopItems as si left outer join
	Items as i on si.ItemId = i.ItemId left outer join
	Categories as c on i.CategoryId = c.CategoryId left outer join
	Shops as s on si.ShopId = s.ShopId left outer join
	Taxs as t on si.TaxId = t.TaxId left outer join
	Folders as fo on convert(varchar(38), si.ShopItemId) = fo.Name left outer join
	UnitGroups as ug on si.UnitGroupId = ug.UnitGroupId left outer join
	Units as u on ug.UnitGroupId = u.UnitGroupId

	where si.ShopItemStatusId = 1 
	and 
	(
		@SearchText is null 
		or @SearchText = '' 
		or i.Name like '%@' + @SearchText + '%' 
		or i.Code like '%@' + @SearchText + '%' 
		or c.Name like '%@' + @SearchText + '%'
		or s.Name like '%@' + @SearchText + '%'
	)
	and 
	(
		@CategoryIds is null 
		or @CategoryIds = '' 
		or c.CategoryId in (select CategoryId from @CategoriesIds)
	)
	and 
	(
		@ShopIds is null 
		or @ShopIds = '' 
		or s.ShopId in (select ShopId from @ShopeIds)
	)

	order by Category
	OFFSET (@PageNumber-1) * @PageSize rows
	fetch next @PageSize rows ONLY
	) 

	select 
	
	top 100

		si.ShopItemId,
		si.Name,
		si.NameAr,
		si.Code,
		si.Descreption,
		si.Price,
		si.Category,
		si.Quantity,
		si.Rating,
		si.IsInFavorite,
		si.Shope,
		si.ShopId,
		si.Discount,
		si.Tax,
		si.SelectedUnitId,
		si.UnitGroupId,
		si.ImagesPaths,
		(case when count(si.UnitId) = 0 
			  then '' 
			  else STRING_AGG(si.UnitId,';') 
			  end 
		) as UnitIds,
		(case when count(si.UnitName) = 0 
			  then '' 
			  else STRING_AGG(si.UnitName,';') 
			  end 
		) as UnitNames,
		null as UnitModels

	from ShopItemData as si

	group by 

	si.ShopItemId,
	si.Name,
	si.NameAr,
	si.Code,
	si.Descreption,
	si.Price,
	si.Category,
	si.Quantity,
	si.Rating,
	si.IsInFavorite,
	si.Shope,
	si.ShopId,
	si.Discount,
	si.Tax,
	si.SelectedUnitId,
	si.UnitGroupId,
	si.ImagesPaths
END
