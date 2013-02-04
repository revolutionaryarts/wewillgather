using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Users;

namespace Gather.Data.Mapping.Users
{
    public class UserRoleMap : EntityTypeConfiguration<UserRole>
    {
        public UserRoleMap()
        {
            ToTable("UserRole");
            HasKey(x => x.Id);
            Property(x => x.Name).IsRequired().HasMaxLength(255);
            Property(x => x.SystemName).IsRequired().HasMaxLength(255);
        }
    }
}