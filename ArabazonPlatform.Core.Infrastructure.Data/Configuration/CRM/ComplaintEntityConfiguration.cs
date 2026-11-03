using ArabazonPlatform.Domain.Domains.CMS;
using ArabazonPlatform.Domain.Domains.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.CRM
{
    public class ComplaintEntityConfiguration : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> builder)
        {
            builder.ToTable("Complaints");
            builder.HasKey(e => e.ComplaintId);

            #region Foreign keys      
            builder.HasOne(i => i.Customer).WithMany(i => i.Complaints).HasForeignKey(k => k.CustomerId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(i => i.User).WithMany(i => i.Complaints).HasForeignKey(k => k.UserId).OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
