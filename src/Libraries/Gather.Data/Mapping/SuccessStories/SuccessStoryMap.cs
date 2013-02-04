using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.SuccessStories;

namespace Gather.Data.Mapping.SuccessStories
{
    public class SuccessStoryMap : EntityTypeConfiguration<SuccessStory>
    {
        public SuccessStoryMap()
        {
            ToTable("SuccessStory");
            HasKey(x => x.Id);
            Property(x => x.Title).HasMaxLength(100);
            Property(x => x.ShortSummary).HasMaxLength(400);
            Property(x => x.Article).IsMaxLength();
            Property(x => x.MetaTitle).HasMaxLength(100);
            Property(x => x.MetaKeywords).HasMaxLength(500);
            Property(x => x.MetaDescription).HasMaxLength(400);
            HasRequired(x => x.Author);
        }
    }
}