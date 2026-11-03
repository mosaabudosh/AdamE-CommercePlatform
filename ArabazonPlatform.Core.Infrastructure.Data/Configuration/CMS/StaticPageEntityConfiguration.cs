using ArabazonPlatform.Domain.Domains.CMS;
using ArabazonPlatform.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.CMS
{
    public class StaticPageEntityConfiguration : IEntityTypeConfiguration<StaticPage>
    {
        public void Configure(EntityTypeBuilder<StaticPage> builder)
        {
            builder.ToTable("StaticPages");
            builder.HasKey(e => e.StaticPageId);

            #region Seeding Data
            builder.HasData(new StaticPage[]
            {
                new StaticPage()
                { 
                    StaticPageId = (int)StaticPages.AboutUs,
                    Name = "About Us" , 
                    NameAr = "معلومات عنا",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new StaticPage()
                {
                    StaticPageId = (int)StaticPages.PrivacyPolicy,
                    Name = "Privacy Policy" ,
                    NameAr = "سياسة الخصوصية",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new StaticPage()
                {
                    StaticPageId = (int)StaticPages.ConsumerRights,
                    Name = "Consumer Rights" ,
                    NameAr = "حقوق المستهلك",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
            });
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
