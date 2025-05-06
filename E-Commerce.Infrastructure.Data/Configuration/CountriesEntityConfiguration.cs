

using E_Commerce.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration
{
	public class CountriesEntityConfiguration : IEntityTypeConfiguration<Country>
	{
		public void Configure(EntityTypeBuilder<Country> builder)
		{
			builder.ToTable("Countries");
			builder.HasKey(e => e.CountryId);

            #region Seeding
            builder.HasData(new Country 
			{ 
				CountryId = 1,
				Name = "Jordan",
				NameAr = "الأردن",
				CreationDate = new DateTime(2025, 1, 1),
				IsActive = true
            });
            #endregion
        }
    }
}
