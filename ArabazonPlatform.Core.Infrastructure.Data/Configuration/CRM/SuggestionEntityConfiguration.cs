using ArabazonPlatform.Domain.Domains.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.CRM
{
    public class SuggestionEntityConfiguration : IEntityTypeConfiguration<Suggestion>
    {
        public void Configure(EntityTypeBuilder<Suggestion> builder)
        {
            builder.ToTable("Suggestions");
            builder.HasKey(e => e.SuggestionId);

            #region Foreign keys      
            builder.HasOne(i => i.Customer).WithMany(i => i.Suggestions).HasForeignKey(k => k.CustomerId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(i => i.User).WithMany(i => i.Suggestions).HasForeignKey(k => k.UserId).OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
