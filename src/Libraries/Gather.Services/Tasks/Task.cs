using System;
using System.Diagnostics;
using Gather.Core.Domain.Tasks;
using Gather.Core.Infrastructure;
using Gather.Services.Logging;

namespace Gather.Services.Tasks
{
    /// <summary>
    /// Task
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Ctor for Task
        /// </summary>
        private Task()
        {
            Enabled = true;
        }

        /// <summary>
        /// Ctor for Task
        /// </summary>
        /// <param name="task">Task </param>
        public Task(ScheduleTask task)
        {
            Enabled = task.Enabled;
            FirstRun = true;
            Name = task.Name;
            RunOnLoad = task.RunOnLoad;
            StopOnError = task.StopOnError;
            Type = task.Type;
        }

        private ITask CreateTask()
        {
            ITask task = null;

            if (Enabled)
            {
                var type2 = System.Type.GetType(Type);
                if (type2 != null)
                    task = Activator.CreateInstance(type2) as ITask;
            }

            return task;
        }
        
        /// <summary>
        /// Executes the task
        /// </summary>
        public void Execute()
        {
            IsRunning = true;

            if (!FirstRun || RunOnLoad)
            {
                try
                {
                    var task = CreateTask();
                    if (task != null)
                    {
                        LastStartUtc = DateTime.UtcNow;
                        task.Execute();
                        LastEndUtc = LastSuccessUtc = DateTime.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    Enabled = !StopOnError;
                    LastEndUtc = DateTime.UtcNow;

                    // Log error
                    var logger = EngineContext.Current.Resolve<ILogService>();
                    logger.Error(string.Format("Error while running the '{0}' schedule task. {1}", Name, ex.Message), ex);
                }

                try
                {
                    // Find current schedule task
                    var scheduleTaskService = EngineContext.Current.Resolve<IScheduleTaskService>();
                    var scheduleTask = scheduleTaskService.GetTaskByType(Type);
                    if (scheduleTask != null)
                    {
                        scheduleTask.LastStartUtc = LastStartUtc;
                        scheduleTask.LastEndUtc = LastEndUtc;
                        scheduleTask.LastSuccessUtc = LastSuccessUtc;
                        scheduleTaskService.UpdateTask(scheduleTask);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("Error saving schedule task datetimes. Exception: {0}", ex));
                }
            }

            FirstRun = false;
            IsRunning = false;
        }

        /// <summary>
        /// A value indicating whether the task is enabled
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// A value indicating if this is the first time this task has run
        /// </summary>
        public bool FirstRun { get; private set; }

        /// <summary>
        /// A value indicating whether a task is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Datetime of the last start
        /// </summary>
        public DateTime? LastStartUtc { get; private set; }

        /// <summary>
        /// Datetime of the last end
        /// </summary>
        public DateTime? LastEndUtc { get; private set; }

        /// <summary>
        /// Datetime of the last success
        /// </summary>
        public DateTime? LastSuccessUtc { get; private set; }

        /// <summary>
        /// Get the task name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A value indicating whether to run the task on first load
        /// </summary>
        public bool RunOnLoad { get; private set; }

        /// <summary>
        /// A value indicating whether to stop task on error
        /// </summary>
        public bool StopOnError { get; private set; }

        /// <summary>
        /// A value indicating type of the task
        /// </summary>
        public string Type { get; private set; }
    }
}