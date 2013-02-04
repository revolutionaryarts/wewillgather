using Gather.Core.Domain.ModerationQueue;
using System.Data.Entity.ModelConfiguration;

namespace Gather.Data.Mapping.ModerationQueues
{
    public class ProjectApprovalMap : EntityTypeConfiguration<ProjectApproval>
    {
        public ProjectApprovalMap()
        {
            ToTable("Moderation_ProjectApproval");
            HasKey(x => x.Id);            
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);

            HasRequired(x => x.ModerationQueue);
        }
    }
}