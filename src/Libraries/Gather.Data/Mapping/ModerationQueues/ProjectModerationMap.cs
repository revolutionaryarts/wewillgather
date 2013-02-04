
using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.ModerationQueue;

namespace Gather.Data.Mapping.ModerationQueues
{
    public class ProjectModerationMap : EntityTypeConfiguration<ProjectModeration>
    {
        public ProjectModerationMap()
        {
            ToTable("Moderation_ProjectContent");
            HasKey(x => x.Id);
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);
            Property(x => x.Reason).HasMaxLength(255);

            HasRequired(x => x.ModerationQueue);
        }
    }
}
