using E_Commerce.Domain.Domains;
using E_Commerce.Domain.Domains.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.Settings
{
    public class DataTypeEntityConfiguration : IEntityTypeConfiguration<DataType>
    {
        public void Configure(EntityTypeBuilder<DataType> builder)
        {
            builder.ToTable("DataTypes");
            builder.HasKey(e => e.DataTypeId);

            #region Seeding Data
            builder.HasData(new DataType[]
            {
                new DataType(){ DataTypeId = (int)DataTypes.Integer, Name = "Integer" , NameAr = "عدد صحيح"},
                new DataType(){ DataTypeId = (int)DataTypes.Decimal, Name = "Decimal" , NameAr = "عدد عشري"},
                new DataType(){ DataTypeId = (int)DataTypes.String, Name = "String" , NameAr = "سلسلة"},
                new DataType(){ DataTypeId = (int)DataTypes.Boolean, Name = "Boolean" , NameAr = "منطقي"},
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
