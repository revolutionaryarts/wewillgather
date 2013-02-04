using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Messages;

namespace Gather.Data.Mapping.Message
{
    public class MessageQueueMap : EntityTypeConfiguration<MessageQueue>
    {
        public MessageQueueMap()
        {
            ToTable("MessageQueue");
            HasKey(x => x.Id);
            Ignore(x => x.Active);
            Property(x => x.ShortBody).HasMaxLength(140);
            HasOptional(x => x.User).WithMany().WillCascadeOnDelete(true);
        }
    }
}
