using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Logging;

namespace Gather.Data.Mapping.Logging
{
    public class LogMap : EntityTypeConfiguration<Log>
    {
        public LogMap()
        {
            ToTable("Log");
            HasKey(l => l.Id);
            Property(l => l.ShortMessage).IsRequired().IsMaxLength();
            Property(l => l.FullMessage).IsMaxLength();
            Property(l => l.IpAddress).HasMaxLength(200);

            Ignore(l => l.LogLevel);

            HasOptional(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
            .WillCascadeOnDelete(true);

        }
    }
}
