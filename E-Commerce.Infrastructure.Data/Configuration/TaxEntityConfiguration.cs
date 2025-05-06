
using E_Commerce.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration
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
