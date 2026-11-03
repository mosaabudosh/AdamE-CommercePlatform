using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using File = ArabazonPlatform.Domain.Domains.DMS.File;


namespace ArabazonPlatform.Infrastructure.Data.Configuration.DMS
{
    public class FileEntityConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            builder.ToTable("Files");
            builder.HasKey(p => p.FileId);

            #region Foreign keys      
            builder.HasOne(i => i.Folder).WithMany(i => i.Files).HasForeignKey(k => k.FolderId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(i => i.FileType).WithMany(i => i.Files).HasForeignKey(k => k.FileTypeId);
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            builder.Ignore(p => p.CreationDate);
            builder.Ignore(p => p.CreationUserId);
            builder.Ignore(p => p.IsActive);
            #endregion
        }
    }
}
