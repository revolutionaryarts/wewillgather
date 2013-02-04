using System;
using Autofac;
using Gather.Core.Infrastructure.DependencyManagement;

namespace Gather.Core.Infrastructure
{
    public class Engine : IEngine
    {

        #region Fields

        private readonly ContainerManager _containerManager;

        #endregion

        #region Constructors

        public Engine()
        {
            var builder = new ContainerBuilder();
            var configurer = new ContainerConfigurer();

            _containerManager = new ContainerManager(builder.Build());
            configurer.Configure(this, _containerManager);
        }

        #endregion

        #region Methods

        public T Resolve<T>() where T : class
        {
            return ContainerManager.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public Array ResolveAll(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        #endregion

        #region Properties

        public IContainer Container
        {
            get { return _containerManager.Container; }
        }

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion

    }
}