using ArabazonPlatform.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard
{
    public class LanguageEntityConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Languages");
            builder.HasKey(e => e.LanguageId);

            #region Seeding Data
            builder.HasData(new Language[]
            {
                new Language()
                {
                    LanguageId = (int)Languages.English,
                    Name = "English" ,
                    NameAr = "الإنجليزية",
                    Code = ArabazonPlatform.Generic.Constants.Constants.Languages.LanguageCodeEn
                },
                new Language()
                {
                    LanguageId = (int)Languages.Arabic,
                    Name = "Arabic" ,
                    NameAr = "العربية",
                    Code = ArabazonPlatform.Generic.Constants.Constants.Languages.LanguageCodeAr
                },
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
