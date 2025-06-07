using E_Commerce.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration.DashBoard
{
    public class ItemEntityConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Items");
            builder.HasKey(e => e.ItemId);

            #region Foreignkeys
            builder.HasOne(e => e.Category).WithMany(e => e.Items).HasForeignKey(f => f.CategoryId);
            builder.HasOne(e => e.UnitGroup).WithMany(e => e.Items).HasForeignKey(f => f.UnitGroupId).OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
