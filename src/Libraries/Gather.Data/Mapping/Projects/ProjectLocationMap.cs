using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Projects;

namespace Gather.Data.Mapping.Projects
{
    public class ProjectLocationMap : EntityTypeConfiguration<ProjectLocation>
    {
        public ProjectLocationMap()
        {
            ToTable("Project_Location_Mapping");
            HasKey(pl => pl.Id);

            Ignore(pl => pl.Active);
            Ignore(pl => pl.Deleted);

            HasRequired(pl => pl.Location)
                .WithMany()
                .HasForeignKey(pl => pl.LocationId);

            HasRequired(pl => pl.Project)
                .WithMany(p => p.Locations)
                .HasForeignKey(pl => pl.ProjectId);
        }       
    }
}