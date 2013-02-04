using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Projects;

namespace Gather.Data.Mapping.Projects
{
    public class ProjecUserHistoryMap : EntityTypeConfiguration<ProjectUserHistory>
    {
        public ProjecUserHistoryMap()
        {
            ToTable("ProjectUserHistory");
            HasKey(x => x.Id);
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);

            HasRequired(x => x.AffectedUser);
            HasRequired(x => x.Project);

        }
    }
}