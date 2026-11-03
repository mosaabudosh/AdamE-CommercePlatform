
using ArabazonPlatform.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.Authorisation
{
	public class RoleScreenActionEntityConfiguration : IEntityTypeConfiguration<RoleScreenAction>
	{
		public void Configure(EntityTypeBuilder<RoleScreenAction> builder)
		{
			builder.ToTable("RoleScreenActions");
			builder.HasKey(e => e.RoleScreenActionId);

			#region Foreignkeys
			builder.HasOne(e => e.RoleScreen).WithMany(e => e.RoleScreenActions).HasForeignKey(f => f.RoleScreenId);
			builder.HasOne(e => e.Action).WithMany(e => e.RoleScreenActions).HasForeignKey(f => f.ActionId);
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
