using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Gather.Core.Domain.Tasks;

namespace Gather.Services.Tasks
{
    /// <summary>
    /// Represents task thread
    /// </summary>
    public class TaskThread : IDisposable
    {
        private Timer _timer;
        private bool _disposed;
        private readonly Dictionary<string, Task> _tasks;

        internal TaskThread()
        {
            _tasks = new Dictionary<string, Task>();
            Seconds = 10 * 60;
        }

        internal TaskThread(ScheduleTask scheduleTask)
        {
            _tasks = new Dictionary<string, Task>();
            Seconds = scheduleTask.Seconds;
            IsRunning = false;
        }

        private void Run()
        {
            if (Seconds <= 0)
                return;

            Started = DateTime.UtcNow;
            IsRunning = true;

            foreach (var task in _tasks.Values)
                task.Execute();

            IsRunning = false;
        }

        private void TimerHandler(object state)
        {
            _timer.Change(-1, -1);
            Run();
            _timer.Change(Interval, Interval);
        }

        /// <summary>
        /// Disposes the instance
        /// </summary>
        public void Dispose()
        {
            if (_timer == null || _disposed) 
                return;

            lock (this)
            {
                _timer.Dispose();
                _timer = null;
                _disposed = true;
            }
        }

        /// <summary>
        /// Inits a timer
        /// </summary>
        public void InitTimer()
        {
            if (_timer == null)
            {
                _timer = new Timer(TimerHandler, null, 10000, Interval);
            }
        }

        /// <summary>
        /// Adds a task to the thread
        /// </summary>
        /// <param name="task">The task to be added</param>
        public void AddTask(Task task)
        {
            if (!_tasks.ContainsKey(task.Name))
                _tasks.Add(task.Name, task);
        }

        /// <summary>
        /// Gets or sets the interval in seconds at which to run the tasks
        /// </summary>
        public int Seconds { get; internal set; }

        /// <summary>
        /// Get a datetime when thread has been started
        /// </summary>
        public DateTime Started { get; private set; }

        /// <summary>
        /// Get a value indicating whether thread is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Get a list of tasks
        /// </summary>
        public IList<Task> Tasks
        {
            get
            {
                var list = _tasks.Values.ToList();
                return new ReadOnlyCollection<Task>(list);
            }
        }

        /// <summary>
        /// Gets the interval at which to run the tasks
        /// </summary>
        public int Interval
        {
            get { return Seconds*1000; }
        }
    }
}