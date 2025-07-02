

Create or ALTER PROCEDURE [dbo].[SP_FeaturedProducts]
	@PageNumber int = null,
	@PageSize int = null 
AS
BEGIN
	set @PageSize = ISNULL(@PageSize, 2147483647)
	set @PageNumber = ISNULL(@PageNumber, 1)

	select 
		si.ShopItemId,
		i.Name,
		i.NameAr,
		i.Code,
		i.Descreption,
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

	order by Category
	OFFSET (@PageNumber-1) * @PageSize rows
	fetch next @PageSize rows ONLY;
END
