

using E_Commerce.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration.Authorisation
{
	public class TempCustomerEntityConfiguration : IEntityTypeConfiguration<TempCustomer>
	{
		public void Configure(EntityTypeBuilder<TempCustomer> builder)
		{
			builder.ToTable("TempCustomers");
			builder.HasKey(e => e.TempCustomerId);
		}
	}
}
