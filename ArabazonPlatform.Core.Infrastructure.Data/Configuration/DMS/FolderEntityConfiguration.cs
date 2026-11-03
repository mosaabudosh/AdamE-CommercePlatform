using ArabazonPlatform.Domain.Domains.DMS;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace ArabazonPlatform.Infrastructure.Data.Configuration.DMS
{
    public class FolderEntityConfiguration : IEntityTypeConfiguration<Folder>
    {
        public void Configure(EntityTypeBuilder<Folder> builder)
        {
            builder.ToTable("Folders");
            builder.HasKey(p => p.FolderId);

            #region Foreign keys      
            builder.HasOne(i => i.ParentFolder).WithMany(i => i.ChildFolders).HasForeignKey(k => k.ParentFolderId).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Ignore
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            builder.Ignore(x => x.LastUpdateUserId);
            builder.Ignore(x => x.LastUpdatedDate);
            #endregion
        }
    }
}
