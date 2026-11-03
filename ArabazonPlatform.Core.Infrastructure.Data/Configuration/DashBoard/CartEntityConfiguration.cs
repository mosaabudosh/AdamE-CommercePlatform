using ArabazonPlatform.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard
{
    public class CartEntityConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");
            builder.HasKey(e => e.CartId);

            #region Foreignkeys
            builder.HasOne(e => e.CartStatus).WithMany(e => e.Carts).HasForeignKey(f => f.CartStatusId);
            builder.HasOne(e => e.Customer).WithMany(e => e.Carts).HasForeignKey(f => f.CustomerId);
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
