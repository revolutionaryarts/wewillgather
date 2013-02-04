using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Pages;

namespace Gather.Data.Mapping.Pages
{
    public class PageMap : EntityTypeConfiguration<Page>
    {
        public PageMap()
        {
            ToTable("Page");
            HasKey(x => x.Id);
            Property(x => x.Title).HasMaxLength(100);
            Property(x => x.Content).IsMaxLength();
            Property(x => x.MetaTitle).HasMaxLength(100);
            Property(x => x.MetaKeywords).HasMaxLength(500);
            Property(x => x.MetaDescription).HasMaxLength(400);
            Property(x => x.FileTitle).HasMaxLength(100);
        }
    }
}