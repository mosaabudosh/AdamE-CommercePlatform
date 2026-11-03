using ArabazonPlatform.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;
using ArabazonPlatform.Domain.Domains.DashBoard;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard
{
    public class PaymentMethodEntityConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("PaymentMethods");
            builder.HasKey(e => e.PaymentMethodId);
        }
    }
}
