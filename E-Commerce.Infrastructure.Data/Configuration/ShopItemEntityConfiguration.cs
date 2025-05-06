
using E_Commerce.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration
{
	public class ShopItemEntityConfiguration : IEntityTypeConfiguration<ShopItem>
	{
		public void Configure(EntityTypeBuilder<ShopItem> builder)
		{
			builder.ToTable("ShopItems");
			builder.HasKey(e => e.ShopItemId);

			#region Foreignkeys
			builder.HasOne(e => e.Tax).WithMany(e => e.ShopItems).HasForeignKey(f => f.TaxId);
			builder.HasOne(e => e.Item).WithOne(e => e.ShopItem).HasForeignKey<ShopItem>(f => f.ItemId);
			#endregion
		}
	}
}
