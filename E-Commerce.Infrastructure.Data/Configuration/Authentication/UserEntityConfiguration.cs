
using E_Commerce.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static E_Commerce.Domain.Enum.Enums;
using System.Text;
using E_Commerce.Domain.Enum;
using static Adam.Generic.Constants.Constants;
using Microsoft.AspNetCore.Identity;

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
			builder.HasOne(e => e.Gender).WithMany(e => e.Users).HasForeignKey(f => f.GenderId);
			builder.HasOne(e => e.City).WithMany(e => e.Users).HasForeignKey(f => f.CityId);
			builder.HasOne(e => e.Shop).WithMany(e => e.Users).HasForeignKey(f => f.ShopId);
            #endregion

            #region Seeding
            byte[] passwordHash;
            byte[] passwordSalt;
            using (System.Security.Cryptography.HMACSHA512 HMACSHA512 = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = HMACSHA512.Key;
                passwordHash = HMACSHA512.ComputeHash(System.Text.Encoding.UTF8.GetBytes("123456"));
            }
            Guid userId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E");
            builder.HasData(new User[] {
                new User()
                {
                      UserId = userId,
                      UserName = "Admin",
                      PasswordHash = passwordHash,
                      PasswordSalt = passwordSalt,
                      FirstName = "Admin",
                      FirstNameAr = "أدمن",
                      SecondName = "Admin",
                      SecondNameAr = "أدمن",
                      LastName = "Admin",
                      LastNameAr = "أدمن",
                      Email = "Admin@gmail.com",
                      MobileNumber = "0788647816",
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
