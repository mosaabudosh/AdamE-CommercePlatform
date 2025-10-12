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
    public class BrandEntityConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brands");
            builder.HasKey(e => e.BrandId);

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
