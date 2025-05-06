

using E_Commerce.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration
{
	public class ItemEntityConfiguration : IEntityTypeConfiguration<Item>
	{
		public void Configure(EntityTypeBuilder<Item> builder)
		{
			builder.ToTable("Items");
			builder.HasKey(e => e.ItemId);

			#region Foreignkeys
			builder.HasOne(e => e.Category).WithMany(e => e.Items).HasForeignKey(f => f.CategoryId);
			#endregion
		}
	}
}
