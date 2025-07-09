using E_Commerce.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Data.Configuration.DashBoard
{
    public class ColorGroupEntityConfiguration : IEntityTypeConfiguration<ColorGroup>
    {
        public void Configure(EntityTypeBuilder<ColorGroup> builder)
        {
            builder.ToTable("ColorGroups");
            builder.HasKey(e => e.ColorGroupId);
        }
    }
}
