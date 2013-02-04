using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.MediaFile;

namespace Gather.Data.Mapping.MediaFile
{
    public class MediaMap : EntityTypeConfiguration<Media>
    {
        public MediaMap()
        {
            ToTable("Media");
            HasKey(x => x.Id);
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);
            Ignore(x => x.FileType);
            Property(x => x.FileName).HasMaxLength(255); 
            Property(x => x.Name).HasMaxLength(255);
        }
    }
}