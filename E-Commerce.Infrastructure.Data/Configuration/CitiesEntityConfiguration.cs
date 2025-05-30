

using E_Commerce.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration
{
	public class CitiesEntityConfiguration : IEntityTypeConfiguration<City>
	{
		public void Configure(EntityTypeBuilder<City> builder)
		{
			builder.ToTable("Cities");
			builder.HasKey(e => e.CityId);

			#region Foreignkeys
			builder.HasOne(e => e.Country).WithMany(e => e.Cities).HasForeignKey(f => f.CountryId);
            #endregion

            #region Seeding
            builder.HasData(new City[] {
              new City 
              { 
                  CityId = 1, 
                  Name = "Amman", 
                  NameAr = "عمان", 
                  CountryId = 1,
                  CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                  CreationDate = new DateTime(2025,1,1), 
                  IsActive = true 
              },
              new City 
              { 
                  CityId = 2, 
                  Name = "Irbid",
                  NameAr = "اربد", 
                  CountryId = 1,
                  CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                  CreationDate = new DateTime(2025,1,1), 
                  IsActive = true 
              },
              new City 
              { 
                  CityId = 3, 
                  Name = "Zarqa",
                  NameAr = "الزرقاء", 
                  CountryId = 1,
                  CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                  CreationDate = new DateTime(2025,1,1), 
                  IsActive = true 
              },
              new City 
              { 
                  CityId = 4, 
                  Name = "Aqaba", 
                  NameAr = "العقبة", 
                  CountryId = 1,
                  CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                  CreationDate = new DateTime(2025,1,1), 
                  IsActive = true 
              },
              new City 
              { 
                  CityId = 5, 
                  Name = "Madaba", 
                  NameAr = "مأدبا", 
                  CountryId = 1,
                  CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                  CreationDate = new DateTime(2025,1,1), 
                  IsActive = true 
              },
              new City 
              { 
                  CityId = 6, 
                  Name = "Jerash", 
                  NameAr = "جرش", 
                  CountryId = 1,
                  CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                  CreationDate = new DateTime(2025,1,1), 
                  IsActive = true 
              },
              new City 
              { 
                  CityId = 7, 
                  Name = "Salt", 
                  NameAr = "السلط", 
                  CountryId = 1,
                  CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                  CreationDate = new DateTime(2025,1,1), 
                  IsActive = true 
              },
              new City 
              { 
                  CityId = 8, 
                  Name = "Mafraq", 
                  NameAr = "المفرق", 
                  CountryId = 1,
                  CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                  CreationDate = new DateTime(2025,1,1),
                  IsActive = true 
              },
              new City 
              { 
                  CityId = 9, 
                  Name = "Karak", 
                  NameAr = "الكرك", 
                  CountryId = 1,
                  CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                  CreationDate = new DateTime(2025,1,1), 
                  IsActive = true 
              },
              new City 
              { 
                  CityId = 10, 
                  Name = "Tafilah", 
                  NameAr = "الطفيلة", 
                  CountryId = 1,
                  CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                  CreationDate = new DateTime(2025,1,1),
                  IsActive = true 
              },
              new City 
              { 
                  CityId = 11, 
                  Name = "Ma'an",
                  NameAr = "معان", 
                  CountryId = 1,
                  CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                  CreationDate = new DateTime(2025,1,1),
                  IsActive = true 
              },
              new City 
              { 
                  CityId = 12,
                  Name = "Ajloun", 
                  NameAr = "عجلون",
                  CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                  CountryId = 1, 
                  CreationDate = new DateTime(2025,1,1),
                  IsActive = true 
              }
            });
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
