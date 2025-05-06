

using E_Commerce.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration
{
	public class CartDetailEntityConfiguration : IEntityTypeConfiguration<CartDetail>
	{
		public void Configure(EntityTypeBuilder<CartDetail> builder)
		{
			builder.ToTable("CartDetails");
			builder.HasKey(e => e.CartDetailId);

			#region Foreignkeys
			builder.HasOne(e => e.ShopItem).WithOne(e => e.CartDetail).HasForeignKey<CartDetail>(f => f.ShopItemId);
			#endregion
		}
	}
}
