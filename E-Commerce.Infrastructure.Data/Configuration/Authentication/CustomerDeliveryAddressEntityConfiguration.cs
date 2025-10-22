using E_Commerce.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Domains.Authentication;

namespace E_Commerce.Infrastructure.Data.Configuration.Authentication
{
    public class CustomerDeliveryAddressEntityConfiguration : IEntityTypeConfiguration<CustomerDeliveryAddress>
    {
        public void Configure(EntityTypeBuilder<CustomerDeliveryAddress> builder)
        {
            builder.ToTable("CustomerDeliveryAddresses");
            builder.HasKey(e => e.CustomerDeliveryAddressId);

            #region Foreignkeys
            builder.HasOne(e => e.Customer).WithMany(e => e.CustomerDeliveryAddresses).HasForeignKey(f => f.CustomerId);
            #endregion
        }
    }
}
