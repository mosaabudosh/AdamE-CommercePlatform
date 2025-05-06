

using E_Commerce.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration
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
				new Language(){ LanguageId = (int)Languages.English, Name = "English" , NameAr = "الإنجليزية",Code = "en-US"},
				new Language(){ LanguageId = (int)Languages.Arabic, Name = "Arabic" , NameAr = "العربية",Code = "ar-JO"},
			});
			#endregion

			#region NotMapped
			builder.Ignore(p => p.CreationDate);
			builder.Ignore(p => p.CreationUserId);
			builder.Ignore(p => p.LastUpdatedDate);
			builder.Ignore(p => p.LastUpdateUserId);
			builder.Ignore(p => p.IsActive);
			#endregion
		}
	}
}
