using System;
using System.Collections.Generic;
using Gather.ApplicationMonitor.Core.Tasks;
using Gather.ApplicationMonitor.Data;

namespace Gather.ApplicationMonitor.Services
{
    public class ScheduledTaskService : IScheduledTaskService
    {
        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <returns>Tasks</returns>
        public IList<ScheduleTask> GetAllTasks()
        {

            List<ScheduleTask> tasks = new List<ScheduleTask>();

            try
            {
                using (var db = new Database())
                {

                    var sp = new Query("SELECT * FROM dbo.ScheduleTask", db);

                    using (var rs = sp.Get())
                    {
                        while (rs.Read())
                        {
                            var task = new ScheduleTask()
                            {
                                Name = rs.GetString("Name"),
                                Enabled = rs.GetBool("Enabled"),
                                LastEndUtc = rs.GetDateTime("LastEndUtc"),
                                LastStartUtc = rs.GetDateTime("LastStartUtc"),
                                LastSuccessUtc = rs.GetDateTime("LastSuccessUtc"),
                                Seconds = rs.GetInt("Seconds"),
                                StopOnError = rs.GetBool("StopOnError"),
                                Type = rs.GetString("Type")
                            };
                            tasks.Add(task);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return tasks;
        }

    }
}
