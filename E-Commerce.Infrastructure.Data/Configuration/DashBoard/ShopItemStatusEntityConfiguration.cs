using E_Commerce.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.DashBoard
{
    internal class ShopItemStatusEntityConfiguration : IEntityTypeConfiguration<ShopItemStatus>
    {
        public void Configure(EntityTypeBuilder<ShopItemStatus> builder)
        {
            builder.ToTable("ShopItemStatuses");
            builder.HasKey(e => e.ShopItemStatusId);

            #region Seeding Data
            builder.HasData(new ShopItemStatus[]
            {
                new ShopItemStatus(){ ShopItemStatusId = (int)ShopItemStatuses.Entered, Name = "Entered" , NameAr = "تم إدخاله"},
                new ShopItemStatus(){ ShopItemStatusId = (int)ShopItemStatuses.Approved, Name = "Approved" , NameAr = "تمت القبول"},
                new ShopItemStatus(){ ShopItemStatusId = (int)ShopItemStatuses.Rejected, Name = "Rejected" , NameAr = "مرفوض"},
            });
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            builder.Ignore(p => p.CreationDate);
            builder.Ignore(p => p.CreationUserId);
            builder.Ignore(p => p.LastUpdatedDate);
            builder.Ignore(p => p.LastUpdateUserId);
            builder.Ignore(p => p.IsActive);
            #endregion

        }
    }
}
