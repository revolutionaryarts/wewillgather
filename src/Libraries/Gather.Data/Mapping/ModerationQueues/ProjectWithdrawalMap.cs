
using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.ModerationQueue;

namespace Gather.Data.Mapping.ModerationQueues
{
    public class ProjectWithdrawalMap : EntityTypeConfiguration<ProjectWithdrawal>
    {
        public ProjectWithdrawalMap()
        {
            ToTable("Moderation_ProjectWithdrawal");
            HasKey(x => x.Id);
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);
            Property(x => x.Reason).HasMaxLength(255);

            HasRequired(x => x.ModerationQueue);
        }
    }
}
