
using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Tasks;

namespace Gather.Data.Mapping.Tasks
{
    public partial class ScheduleTaskMap : EntityTypeConfiguration<ScheduleTask>
    {
        public ScheduleTaskMap()
        {
            ToTable("ScheduleTask");
            HasKey(t => t.Id);
            Property(t => t.Name).IsRequired();
            Property(t => t.Type).IsRequired();            
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);  
        }
    }
}
