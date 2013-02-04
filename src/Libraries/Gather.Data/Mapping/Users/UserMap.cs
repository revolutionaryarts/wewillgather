using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Users;

namespace Gather.Data.Mapping.Users
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            ToTable("User");
            HasKey(x => x.Id);
            Property(x => x.DisplayName).HasMaxLength(40);
            Property(x => x.Email).HasMaxLength(1000);
            Property(x => x.FacebookAccessToken).HasMaxLength(200);
            Property(x => x.FacebookProfile).HasMaxLength(1000);
            Property(x => x.Telephone).HasMaxLength(30);
            Property(x => x.TwitterAccessSecret).HasMaxLength(200);
            Property(x => x.TwitterAccessToken).HasMaxLength(200);            
            Property(x => x.TwitterProfile).HasMaxLength(1000);
            Property(x => x.UserName).HasMaxLength(50);
            Property(x => x.Website).HasMaxLength(300);
            Ignore(x => x.Deleted);
            Ignore(x => x.EmailDisclosureLevel);
            Ignore(x => x.TelephoneDisclosureLevel);
            Ignore(x => x.WebsiteDisclosureLevel);
            Ignore(x => x.UserGuid);

            HasMany(x => x.UserRoles)
                .WithMany()
                .Map(m => m.ToTable("User_UserRole_Mapping"));
        }
    }
}