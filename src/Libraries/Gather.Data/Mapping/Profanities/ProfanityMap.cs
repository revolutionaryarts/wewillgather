using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Profanity;

namespace Gather.Data.Mapping.Profanities
{
    public class ProfanityMap : EntityTypeConfiguration<Profanity>
    {
        public ProfanityMap()
        {
            ToTable("Profanity");
            HasKey(x => x.Id);
            Property(x => x.Word).HasMaxLength(100);
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);
        }
    }
}