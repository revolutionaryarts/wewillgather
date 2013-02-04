using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.ModerationQueue;

namespace Gather.Data.Mapping.ModerationQueues
{
    public class ProjectCommentModerationMap : EntityTypeConfiguration<ProjectCommentModeration>
    {
        public ProjectCommentModerationMap()
        {
            ToTable("Moderation_ProjectComment");
            HasKey(x => x.Id);            
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);
            Property(x => x.Reason).HasMaxLength(255);

            HasRequired(x => x.ModerationQueue); 
            HasRequired(x => x.Comment);
        }  
    }
}