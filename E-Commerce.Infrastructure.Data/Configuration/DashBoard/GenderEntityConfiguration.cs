using E_Commerce.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.DashBoard
{
    public class GenderEntityConfiguration : IEntityTypeConfiguration<Gender>
    {
        public void Configure(EntityTypeBuilder<Gender> builder)
        {
            builder.ToTable("Genders");
            builder.HasKey(e => e.GenderId);

            #region Seeding Data
            builder.HasData(new Gender[]
            {
                new Gender(){ GenderId = (int)Genders.Man, Name = "Man" , NameAr = "رجل"},
                new Gender(){ GenderId = (int)Genders.Woman, Name = "Woman" , NameAr = "المرأة"},
            });
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            builder.Ignore(p => p.CreationDate);
            builder.Ignore(p => p.CreationUserId);
            builder.Ignore(p => p.LastUpdatedDate);
            builder.Ignore(p => p.LastUpdateUserId);
            builder.Ignore(p => p.IsActive);
            #endregion
        }
    }
}
