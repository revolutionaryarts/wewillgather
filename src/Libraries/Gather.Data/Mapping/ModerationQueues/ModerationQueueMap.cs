using Gather.Core.Domain.ModerationQueue;
using System.Data.Entity.ModelConfiguration;

namespace Gather.Data.Mapping.ModerationQueues
{
    public class ModerationQueueMap : EntityTypeConfiguration<ModerationQueue>
    {
        public ModerationQueueMap()
        {
            ToTable("ModerationQueue");
            HasKey(x => x.Id);
            Property(x => x.Notes).IsMaxLength();
            Ignore(x => x.Active);
        }
    }
}
