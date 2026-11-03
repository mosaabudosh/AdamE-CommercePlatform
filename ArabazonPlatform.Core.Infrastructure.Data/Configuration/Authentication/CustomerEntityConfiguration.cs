

using ArabazonPlatform.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.Authorisation
{
	public class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
	{
		public void Configure(EntityTypeBuilder<Customer> builder)
		{
			builder.ToTable("Customers");
			builder.HasKey(e => e.CustomerId);

            #region Foreignkeys
            builder.HasOne(e => e.Gender).WithMany(e => e.Customers).HasForeignKey(f => f.GenderId);
            builder.HasOne(e => e.City).WithMany(e => e.Customers).HasForeignKey(f => f.CityId);
            #endregion

            #region Unique
            builder.HasIndex(p => p.UserName).IsUnique();
            #endregion
        }
    }
}
