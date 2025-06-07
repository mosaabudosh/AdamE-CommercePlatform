using E_Commerce.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration.DashBoard
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
                CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                CreationDate = new DateTime(2025, 1, 1),
                IsActive = true
            });
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
