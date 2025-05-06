

using E_Commerce.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration
{
	public class CategoriesEntityConfiguration : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			builder.ToTable("Categories");
			builder.HasKey(e => e.CategoryId);

			#region Foreignkeys
			builder.HasOne(e => e.ParentCategory).WithMany(e => e.ChildCategories).HasForeignKey(f => f.ParentCategoryId);
			#endregion
		}
	}
}
