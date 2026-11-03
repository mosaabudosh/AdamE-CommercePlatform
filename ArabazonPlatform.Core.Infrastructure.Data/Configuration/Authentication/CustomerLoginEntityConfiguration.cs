using ArabazonPlatform.Domain.Domains;
using ArabazonPlatform.Domain.Domains.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.Authentication
{
    public class CustomerLoginEntityConfiguration : IEntityTypeConfiguration<CustomerLogin>
    {
        public void Configure(EntityTypeBuilder<CustomerLogin> builder)
        {
            builder.ToTable("CustomerLogins");
            builder.HasKey(e => e.CustomerLoginId);

            #region Foreignkeys
            builder.HasOne(e => e.Customer).WithMany(e => e.CustomerLogins).HasForeignKey(f => f.CustomerId);
            #endregion
        }
    }
}
