using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Api;

namespace Gather.Data.Mapping.Api
{

    public class ApiAuthenticationMap : EntityTypeConfiguration<ApiAuthentication>
    {
        public ApiAuthenticationMap()
        {
            ToTable("ApiAuthentication");
            HasKey(x => x.Id);
            Property(x => x.NameOfApplication).HasMaxLength(200);
            Property(x => x.WebsiteAddress).HasMaxLength(200);
            Property(x => x.Description).HasMaxLength(400);
            Ignore(x => x.AccessToken);
        }
    }
}
