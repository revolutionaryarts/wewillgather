using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Categories;

namespace Gather.Data.Mapping.Categories
{
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            ToTable("Category");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(100);
        }
    }
}