using System.Web.Http.Dependencies;
using Autofac.Integration.WebApi;
using Gather.Core.Infrastructure;

namespace Gather.Api
{
    public class GatherDependencyResolver : AutofacWebApiDependencyResolver, IDependencyResolver
    {
        public GatherDependencyResolver()
            : base(EngineContext.Current.ContainerManager.Container)
        {

        }

        //public object GetService(Type serviceType)
        //{
        //    try
        //    {
        //        return EngineContext.Current.Resolve(serviceType);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        //public IEnumerable<object> GetServices(Type serviceType)
        //{
        //    try
        //    {
        //        var type = typeof(IEnumerable<>).MakeGenericType(serviceType);
        //        return (IEnumerable<object>)EngineContext.Current.Resolve(type);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
    }
}