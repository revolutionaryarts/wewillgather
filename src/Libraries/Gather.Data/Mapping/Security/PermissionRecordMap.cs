using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Security;

namespace Gather.Data.Mapping.Security
{
    public class PermissionRecordMap : EntityTypeConfiguration<PermissionRecord>
    {
        public PermissionRecordMap()
        {
            ToTable("PermissionRecord");
            HasKey(x => x.Id);
            Property(x => x.Name).IsRequired();
            Property(x => x.SystemName).IsRequired().HasMaxLength(255);
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);

            HasMany(x => x.UserRoles)
                .WithMany(x => x.PermissionRecords)
                .Map(m => m.ToTable("PermissionRecord_Role_Mapping"));
        }
    }
}