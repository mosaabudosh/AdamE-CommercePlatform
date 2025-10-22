using E_Commerce.Domain.Domains.Authentication;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Data.Configuration.Authentication
{
    public class CustomerCardInfoEntityConfiguration : IEntityTypeConfiguration<CustomerCardInfo>
    {
        public void Configure(EntityTypeBuilder<CustomerCardInfo> builder)
        {
            builder.ToTable("CustomerCardInfos");
            builder.HasKey(e => e.CustomerCardInfoId);

            #region Foreignkeys
            builder.HasOne(e => e.Customer).WithMany(e => e.CustomerCardInfos).HasForeignKey(f => f.CustomerId);
            #endregion
        }
    }
}
