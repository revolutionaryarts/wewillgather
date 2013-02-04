using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Locations;

namespace Gather.Data.Mapping.Locations
{
    public class LocationMap : EntityTypeConfiguration<Location>
    {
        public LocationMap()
        {
            ToTable("Location");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(100);
            Property(x => x.HashTag).HasMaxLength(20);
            Property(x => x.Latitude).HasPrecision(25, 18);
            Property(x => x.Longitude).HasPrecision(25, 18);

            HasOptional(x => x.ParentLocation)
                .WithMany(x => x.ChildLocations)
                .Map(m => m.ToTable("Location"));
        }
    }
}