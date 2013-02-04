using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Tweets;

namespace Gather.Data.Mapping.Tweets
{
    public partial class TweetMap : EntityTypeConfiguration<Tweet>
    {
        public TweetMap()
        {
            ToTable("Tweet");
            HasKey(x => x.Id);
            Property(x => x.Text).HasMaxLength(200);
            Property(x => x.TwitterProfile).HasMaxLength(1000);
            Property(x => x.TwitterName).HasMaxLength(1000);
            Property(x => x.UserName).HasMaxLength(1000);
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);  
        }
    }
}
