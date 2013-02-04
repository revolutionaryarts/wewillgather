using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Settings;

namespace Gather.Data.Mapping.Settings
{
    public class SettingMap : EntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            ToTable("Setting");
            HasKey(x => x.Id);
            Property(x => x.Name).IsRequired().HasMaxLength(100);
            Property(x => x.Value).IsRequired().HasMaxLength(2000);
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);
        }
    }
}