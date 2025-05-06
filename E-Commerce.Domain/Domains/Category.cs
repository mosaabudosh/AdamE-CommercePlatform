

namespace E_Commerce.Domain.Domains
{
	public class Category : SharedBase
	{
		public int CategoryId { get; set; }
		public string Name { get; set; }
		public string NameAr { get; set; }
		public string Code { get; set; }
		public string Image { get; set; }
		public string Descreption { get; set; }
		public bool IsBlock { get; set; }
		public int? ParentCategoryId { get; set; }

		public virtual Category ParentCategory { get; set; }
		public virtual List<Category> ChildCategories { get; set; }
		public virtual List<Item> Items { get; set; }
	}
}
