using System.Collections.Generic;
using Gather.ApplicationMonitor.Core.Tasks;

namespace Gather.ApplicationMonitor.Services
{
    public interface IScheduledTaskService
    {
        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <returns>Tasks</returns>
        IList<ScheduleTask> GetAllTasks();
    }
}
