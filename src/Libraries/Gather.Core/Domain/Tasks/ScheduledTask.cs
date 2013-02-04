using System;

namespace Gather.Core.Domain.Tasks
{
    public class ScheduleTask : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the run period (in seconds)
        /// </summary>
        public virtual int Seconds { get; set; }

        /// <summary>
        /// Gets or sets the type of appropriate ITask class
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task is enabled
        /// </summary>
        public virtual bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task should be stopped on some error
        /// </summary>
        public virtual bool StopOnError { get; set; }

        /// <summary>
        /// Gets or sets the last start date
        /// </summary>
        public virtual DateTime? LastStartUtc { get; set; }

        /// <summary>
        /// Gets or sets the last end date
        /// </summary>
        public virtual DateTime? LastEndUtc { get; set; }

        /// <summary>
        /// Gets or sets the last success date
        /// </summary>
        public virtual DateTime? LastSuccessUtc { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating whether to run the task on first load
        /// </summary>
        public virtual bool RunOnLoad { get; set; }
    }
}