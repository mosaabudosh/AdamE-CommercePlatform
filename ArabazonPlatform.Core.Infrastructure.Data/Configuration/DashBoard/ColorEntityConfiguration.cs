using ArabazonPlatform.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard
{
    public class ColorEntityConfiguration : IEntityTypeConfiguration<Color>
    {
        public void Configure(EntityTypeBuilder<Color> builder)
        {
            builder.ToTable("Colors");
            builder.HasKey(e => e.ColorId);

            #region Foreignkeys
            builder.HasOne(e => e.ColorGroup).WithMany(e => e.Colors).HasForeignKey(f => f.ColorGroupId).OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
