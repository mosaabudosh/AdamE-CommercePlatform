using E_Commerce.Domain.Domains.Authentication;
using E_Commerce.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Data.Configuration.DashBoard
{
    public class CustomerFavoriteEntityConfiguration : IEntityTypeConfiguration<CustomerFavorite>
    {
        public void Configure(EntityTypeBuilder<CustomerFavorite> builder)
        {
            builder.ToTable("CustomerFavorites");
            builder.HasKey(e => e.CustomerFavoriteId);

            #region Foreignkeys
            builder.HasOne(e => e.ShopItem).WithMany(e => e.CustomerFavorites).HasForeignKey(f => f.ShopItemId);
            builder.HasOne(e => e.Customer).WithMany(e => e.CustomerFavorites).HasForeignKey(f => f.CustomerId);
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
