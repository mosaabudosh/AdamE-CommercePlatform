using E_Commerce.Domain.Domains.DMS;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Domain.Enum;

namespace E_Commerce.Infrastructure.Data.Configuration.DMS
{
    public class FileTypeEntityConfiguration : IEntityTypeConfiguration<FileType>
    {
        public void Configure(EntityTypeBuilder<FileType> builder)
        {
            builder.ToTable("FileTypes");
            builder.HasKey(sc => sc.FileTypeId);
            builder.Property(sc => sc.FileTypeId).ValueGeneratedNever();

            #region Seeding
            DataBuilder<FileType> dataBuilder = builder.HasData(new FileType[] 
            {
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.PDF,
                    Extension= "pdf",
                    Name = "PDF",
                    ContentType = "application/pdf",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.XML,
                    Extension = "xml",
                    Name = "XML",
                    ContentType = "text/xml",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.ZIP,
                    Extension = "zip",
                    Name = "ZIP",
                    ContentType = "application/zip",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.PNG,
                    Extension = "png",
                    Name = "PNG",
                    ContentType = "image/png",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.JPEG,
                    Extension = "jpeg",
                    Name = "JPEG",
                    ContentType = "image/jpeg",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.MP4,
                    Extension = "mp4",
                    Name = "MP4",
                    ContentType = "video/mp4",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.Word,
                    Extension = "doc",
                    Name = "Word",
                    ContentType = "application/msword",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.PowerPoint,
                    Extension = "ppt",
                    Name = "PowerPoint",
                    ContentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.Xls,
                    Extension = "xls",
                    Name = "Excel",
                    ContentType = "application/vnd.ms-excel",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.Xlsx,
                    Extension = "xlsx",
                    Name = "Excel",
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.Doc,
                    Extension = "doc",
                    Name = "WORD",
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.Docx,
                    Extension = "docx",
                    Name = "WORD",
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.Text,
                    Extension = "txt",
                    Name = "Text",
                    ContentType = "text/plain",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.MP3,
                    Extension = "mp3",
                    Name = "MP3",
                    ContentType = "audio/mp3",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.JPG,
                    Extension = "jpg",
                    Name = "JPG",
                    ContentType = "image/jpg",
                    IsActive = true
                },
                new FileType()
                {
                    FileTypeId = (int)Enums.FileTypes.Worddocx,
                    Extension = "docx",
                    Name = "Word",
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    IsActive = true
                },
                new FileType()
                {
                      FileTypeId = (int)Enums.FileTypes.SVG,
                      Extension = "svg",
                      Name = "SVG",
                      ContentType = "image/svg+xml",
                      IsActive = true
                }, 
                new FileType()
                {
                      FileTypeId = (int)Enums.FileTypes.WEBP,
                      Extension = "webp",
                      Name = "WEBP",
                      ContentType = "image/webp",
                      IsActive = true
                },
                new FileType()
                {
                      FileTypeId = (int)Enums.FileTypes.JFIF,
                      Extension = "jfif",
                      Name = "JFIF",
                      ContentType = "image/jpeg",
                      IsActive = true
                },
                new FileType()
                {
                      FileTypeId = (int)Enums.FileTypes.GIF,
                      Extension = "gif",
                      Name = "GIF",
                      ContentType = "image/gif",
                      IsActive = true
                }
            });
            #endregion

            #region Ignore
            builder.Ignore(x => x.CreationUserId);
            builder.Ignore(x => x.LastUpdateUserId);
            builder.Ignore(x => x.CreationDate);
            builder.Ignore(x => x.LastUpdatedDate);
            #endregion
        }
    }
}
