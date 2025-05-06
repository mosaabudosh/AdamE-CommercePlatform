using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using File = E_Commerce.Domain.Domains.DMS.File;


namespace E_Commerce.Infrastructure.Data.Configuration.DMS
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
        }
    }
}
