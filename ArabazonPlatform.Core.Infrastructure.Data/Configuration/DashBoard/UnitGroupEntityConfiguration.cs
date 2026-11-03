using ArabazonPlatform.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard
{
    public class UnitGroupEntityConfiguration : IEntityTypeConfiguration<UnitGroup>
    {
        public void Configure(EntityTypeBuilder<UnitGroup> builder)
        {
            builder.ToTable("UnitGroups");
            builder.HasKey(e => e.UnitGroupId);
        }
    }
}
