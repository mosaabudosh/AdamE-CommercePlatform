

using E_Commerce.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration
{
	public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.ToTable("Orders");
			builder.HasKey(e => e.OrderId);

			#region Foreignkeys
			builder.HasOne(e => e.OrderStatus).WithMany(e => e.Orders).HasForeignKey(f => f.OrderStatusId);
			builder.HasOne(e => e.User).WithMany(e => e.Orders).HasForeignKey(f => f.UserId);
			#endregion
		}
	}
}
