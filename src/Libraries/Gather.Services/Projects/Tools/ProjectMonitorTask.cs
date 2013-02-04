using Gather.Core.Infrastructure;
using Gather.Services.Tasks;

namespace Gather.Services.Projects.Tools
{
    public class ProjectMonitorTask : ITask
    {
        public void Execute()
        {
            // Initialize engine
            EngineContext.Initialize(false);

            // Resolve project and message queue business layer services
            var projectService = EngineContext.Current.Resolve<IProjectService>();                        

            // Retrieve projects due to start soon. The default is currently 24 hours, despatch the messages and mark as complete.
            projectService.MessageProjectsDueToStart();

            // Under subscribed projects need a tweet 1 hour before project starts to try to obtain more volunteers.
            projectService.MessageProjectsStartingAlertForMoreVolunteers();

            // Mark open projects and set them as in progress
            projectService.UpdateOpenProjectsToInProgress();

            // Mark in progress projects and set them as closed
            projectService.UpdateInProgressProjectsToClosed();

            // Mark projects which were started via twitter but not progressed within 14 days as deleted
            projectService.DeleteDraftProjectsNotProgressed();
        }
    }
}