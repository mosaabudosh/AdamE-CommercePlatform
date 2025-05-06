using E_Commerce.Domain.Domains.DMS;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrastructure.Data.Configuration.DMS
{
    public class FolderConfigurationEntityConfiguration : IEntityTypeConfiguration<FolderConfiguration>
    {
        public void Configure(EntityTypeBuilder<FolderConfiguration> builder)
        {
            builder.ToTable("FolderConfigurations");
            builder.HasKey(p => p.RootFolderPath);
        }
    }
}
