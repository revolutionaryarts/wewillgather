using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Slug;

namespace Gather.Data.Mapping.Slugs
{
    public class SlugMap : EntityTypeConfiguration<Slug>
    {
        public SlugMap()
        {
            ToTable("Slug");
            HasKey(x => x.Id);
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);
            Ignore(x => x.LookupType);
        }
    }
}
