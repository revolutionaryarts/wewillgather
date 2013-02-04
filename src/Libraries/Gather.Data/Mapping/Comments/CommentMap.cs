using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Comments;

namespace Gather.Data.Mapping.Comments
{
    public class CommentMap : EntityTypeConfiguration<Comment>
    {
        public CommentMap()
        {
            ToTable("Comment");
            HasKey(x => x.Id);

            HasRequired(x => x.Author);

            HasOptional(x => x.InResponseTo)
                .WithMany(c => c.Responses);

            HasOptional(x => x.Project)
                .WithMany(p => p.Comments);
        }
    }
}