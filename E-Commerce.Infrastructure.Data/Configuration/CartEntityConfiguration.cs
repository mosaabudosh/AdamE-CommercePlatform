

using E_Commerce.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration
{
	public class CartEntityConfiguration : IEntityTypeConfiguration<Cart>
	{
		public void Configure(EntityTypeBuilder<Cart> builder)
		{
			builder.ToTable("Carts");
			builder.HasKey(e => e.CartId);

			#region Foreignkeys
			builder.HasOne(e => e.CartStatus).WithMany(e => e.Carts).HasForeignKey(f => f.CartStatusId);
			builder.HasOne(e => e.User).WithMany(e => e.Carts).HasForeignKey(f => f.UserId);
			builder.HasOne(e => e.TempCustomer).WithMany(e => e.Carts).HasForeignKey(f => f.TempCustomerId);
			#endregion
		}
	}
}
