
using E_Commerce.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static E_Commerce.Domain.Enum.Enums;
using System.Text;

namespace E_Commerce.Infrastructure.Data.Configuration.Authorisation
{
	public class UserEntityConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("Users");
			builder.HasKey(e => e.UserId);

			#region Foreignkeys
			builder.HasOne(e => e.UserType).WithMany(e => e.Users).HasForeignKey(f => f.UserTypeId);
			builder.HasOne(e => e.Language).WithMany(e => e.Users).HasForeignKey(f => f.LanguageId);
			builder.HasOne(e => e.Gender).WithMany(e => e.Users).HasForeignKey(f => f.GenderId);
			builder.HasOne(e => e.City).WithMany(e => e.Users).HasForeignKey(f => f.CityId);
			builder.HasOne(e => e.Shop).WithMany(e => e.Users).HasForeignKey(f => f.ShopId);
            builder.HasOne(e => e.CreationUser).WithMany(e => e.CreationUsers).HasForeignKey(f => f.CreationUserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.LastUpdateUser).WithMany(e => e.LastUpdateUsers).HasForeignKey(f => f.LastUpdateUserId).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Seeding
            Guid userId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E");
            builder.HasData(new User[] {
                new User()
                {
                      UserId = userId,
                      UserName = "Admin",
                      PasswordHash =  Encoding.ASCII.GetBytes("0x6FAA21E8435DC04FF2C575BE7AB367C3E1B671228874B058B03CCD07B2F91C7D4F59E920D41743E793FFD94BD6E6F05B37B32AD4168382D77E5D1D1CB66B8B4C"),
                      PasswordSalt = Encoding.ASCII.GetBytes("0x7B099B445376E2E135AD1386D38EC88FD9B91233AA8EF44530178FAE81F195F490BC78D4408CAD69150D62F4CF6D023608936082410076F46F849AF67721B8DD041CE86909E894ADDCC37636A542CB3DFEF60F1EC4ACC409960D54BA2C345D3A0543F0355B3BC66B5621310520831D6C3BF54DAFF42AC8963121B7C6FEFA8C27"),
                      FirstName = "Admin",
                      FirstNameAr = "أدمن",
                      SecondName = "Admin",
                      SecondNameAr = "أدمن",
                      LastName = "Admin",
                      LastNameAr = "أدمن",
                      Email = "Admin@gmail.com",
                      MobileNumber = "0788647816",
                      LanguageId = (int)Languages.English,
                      GenderId = (int)Genders.Male,
                      Adresse = "Amman",
                      CityId = 1,
                      DateOfBirth = new DateTime(2025,1,1),
                      IsBlock = false,
                      UserTypeId = (int)UserTypes.OuerTeam,
                      CreationUserId = userId,
                      CreationDate =  new DateTime(2025,1,1),
                      IsActive = true,
                },
            });
            #endregion

            #region Unique
            builder.HasIndex(p => p.UserName).IsUnique();
            #endregion
        }
    }
}
