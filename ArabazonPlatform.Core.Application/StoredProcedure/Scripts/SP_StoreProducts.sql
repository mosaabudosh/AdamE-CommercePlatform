/****** Object:  StoredProcedure [dbo].[SP_StoreProducts]    Script Date: 7/14/2025 4:59:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[SP_StoreProducts]
	@PageNumber int = null,
	@PageSize int = null,
	@SearchText nvarchar(100) = null,
	@CategoryIds nvarchar(100) = null,
	@ShopIds nvarchar(100) = null,
	@IsLoadImage bit = null,
	@FromPrice decimal = null,
	@ToPrice decimal = null

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
		si.Name,
		si.NameAr,
		si.Code,
		si.Descreption,
		(case when d.Value is null then si.Price else si.Price - (si.Price * (d.Value / 100)) end) as Price,
		si.Price as OldPrice,
		c.Name as Category,
		1 as Quantity,
		3 as Rating,
		0 as IsInFavorite,
		s.Name as Shope,
		s.ShopId,
		d.Value as Discount,
		t.Value as Tax,
		si.DefaultUnitId as SelectedUnitId,
		(
			case when @IsLoadImage is not null and @IsLoadImage = 1 
			then (select STRING_AGG(Path,';') from Files where FolderId = fo.FolderId) 
			else null 
			end
		) as ImagesPaths,
		si.UnitGroupId,
		si.ColorGroupId,
		(
			case when @IsLoadImage is not null and @IsLoadImage = 1 
			then s.Logo 
			else null 
			end
		) as ShopeLogo,
		si.DefaultColorId as SelectedColorId,
		si.CategoryId

	from ShopItems as si left outer join
	Categories as c on si.CategoryId = c.CategoryId left outer join
	Shops as s on si.ShopId = s.ShopId left outer join
	Taxs as t on si.TaxId = t.TaxId left outer join
	Folders as fo on convert(varchar(38), si.ShopItemId) = fo.Name left outer join
	UnitGroups as ug on si.UnitGroupId = ug.UnitGroupId left outer join
	ColorGroups as cg on si.ColorGroupId = cg.ColorGroupId left outer join
	Discounts as d on si.DiscountId = d.DiscountId

	
	where si.ShopItemStatusId <> 1 
	and si.IsFeatured = 1
	and 
	(
		@SearchText is null 
		or @SearchText = '' 
		or si.Name like '%@' + @SearchText + '%' 
		or si.Code like '%@' + @SearchText + '%' 
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
	and si.InStocks > 0
	and (@FromPrice is null or si.Price >= @FromPrice)
	and (@ToPrice is null or si.Price <= @ToPrice)




	order by Category
	OFFSET (@PageNumber-1) * @PageSize rows
	fetch next @PageSize rows ONLY
	) 

	select 
		si.ShopItemId,
		si.Name,
		si.NameAr,
		si.Code,
		si.Descreption,
		si.Price,
		si.OldPrice,
		si.Category,
		si.Quantity,
		si.Rating,
		si.IsInFavorite,
		si.Shope,
		si.ShopId,
		si.Discount,
		si.Tax,
		si.SelectedUnitId,
		si.SelectedColorId,
		si.UnitGroupId,
		si.ColorGroupId,
		si.ImagesPaths,
		(select STRING_AGG(Units.UnitId,';')  from Units where Units.UnitGroupId = si.UnitGroupId) as UnitIds,
		(select STRING_AGG(Units.Name,';')  from Units where Units.UnitGroupId = si.UnitGroupId) as UnitNames,
		null as UnitModels,
		(select STRING_AGG(Colors.ColorId,';')  from Colors where Colors.ColorGroupId = si.ColorGroupId) as ColorIds,
		(select STRING_AGG(Colors.Name,';')  from Colors where Colors.ColorGroupId = si.ColorGroupId) as Colors,
		null as ColorModels,
		si.ShopeLogo,
		si.CategoryId

	from ShopItemData as si
	group by 
	si.ShopItemId,
	si.Name,
	si.NameAr,
	si.Code,
	si.Descreption,
	si.Price,
	si.OldPrice,
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
	si.ColorGroupId,
	si.ShopeLogo,
	si.SelectedColorId,
	si.CategoryId
END
