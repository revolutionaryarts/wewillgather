using Gather.Core.Domain.ModerationQueue;
using System.Data.Entity.ModelConfiguration;

namespace Gather.Data.Mapping.ModerationQueues 
{
    public class ProjectChangeRequestMap : EntityTypeConfiguration<ProjectChangeRequest>
    {
        public ProjectChangeRequestMap()
        {
            ToTable("Moderation_ProjectChange");
            HasKey(x => x.Id);            
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);

            HasRequired(x => x.ModerationQueue);
        }
    }
}
