using System;
using System.Linq;

namespace Gather.Core.Infrastructure.DependencyManagement
{
    /// <summary>
    /// Configures the inversion of control container with services
    /// </summary>
    public class ContainerConfigurer
    {
        public virtual void Configure(IEngine engine, ContainerManager containerManager)
        {
            containerManager.AddComponent<IWebHelper, WebHelper>("gather.webHelper");
            containerManager.AddComponent<ITypeFinder, WebAppTypeFinder>("gather.typeFinder");

            // Register all dependency registrars
            var typeFinder = containerManager.Resolve<ITypeFinder>();
            containerManager.UpdateContainer(x =>
            {
                // Find all dependency registrars
                var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
                var drInstances = drTypes.Select(drType => (IDependencyRegistrar)Activator.CreateInstance(drType)).ToList();
                
                // Re-order the registrars to consider any hierarchy
                drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();

                // Loop each registrar, registering the dependencies
                foreach (var dependencyRegistrar in drInstances)
                    dependencyRegistrar.Register(x, typeFinder);
            });

            containerManager.AddComponentInstance<IEngine>(engine, "gather.engine");
            containerManager.AddComponentInstance<ContainerConfigurer>(this, "gather.containerConfigurer");
        }
    }
}