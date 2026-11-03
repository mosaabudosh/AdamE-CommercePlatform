using ArabazonPlatform.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard
{
    public class TaxEntityConfiguration : IEntityTypeConfiguration<Tax>
    {
        public void Configure(EntityTypeBuilder<Tax> builder)
        {
            builder.ToTable("Taxs");
            builder.HasKey(e => e.TaxId);
        }
    }
}
