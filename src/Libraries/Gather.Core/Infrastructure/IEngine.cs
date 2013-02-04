using System;
using Gather.Core.Infrastructure.DependencyManagement;

namespace Gather.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the 
    /// various services composing the #WeWillGather engine. Edit functionality, modules
    /// and implementations access most #WeWillGather functionality through this 
    /// interface.
    /// </summary>
    public interface IEngine
    {
        ContainerManager ContainerManager { get; }

        T Resolve<T>() where T : class;

        object Resolve(Type type);

        Array ResolveAll(Type serviceType);

        T[] ResolveAll<T>();
    }
}