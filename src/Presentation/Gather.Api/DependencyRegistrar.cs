using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.WebApi;
using Gather.Core;
using Gather.Core.Cache;
using Gather.Core.Configuration;
using Gather.Core.Data;
using Gather.Core.Fakes;
using Gather.Core.Infrastructure;
using Gather.Core.Infrastructure.DependencyManagement;
using Gather.Data;
using Gather.Services.ApiAuthentications;
using Gather.Services.Authentication;
using Gather.Services.Categories;
using Gather.Services.Comments;
using Gather.Services.Configuration;
using Gather.Services.Geolocation;
using Gather.Services.Locations;
using Gather.Services.MediaFile;
using Gather.Services.MessageQueues;
using Gather.Services.ModerationQueues;
using Gather.Services.Pages;
using Gather.Services.Projects;
using Gather.Services.Security;
using Gather.Services.Settings;
using Gather.Services.Slugs;
using Gather.Services.SuccessStories;
using Gather.Services.Tweets;
using Gather.Services.Users;

namespace Gather.Api
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            // Register API controllers using assembly scanning.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());            

            // HTTP context
            builder.Register(c => HttpContext.Current != null ? (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) : (new FakeHttpContext("~/") as HttpContextBase)).As<HttpContextBase>().InstancePerApiRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Request).As<HttpRequestBase>().InstancePerApiRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Response).As<HttpResponseBase>().InstancePerApiRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Server).As<HttpServerUtilityBase>().InstancePerApiRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Session).As<HttpSessionStateBase>().InstancePerApiRequest();           

            // Data access
            builder.Register<IDbContext>(c => new GatherObjectContext("DefaultConnection")).InstancePerApiRequest();

            // Repo
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerApiRequest();

            // Cache
            // TODO: Currently disabled because the MemoryCacheManager crashes with Web API. Needs investigation.
            builder.RegisterType<WebAPICacheManager>().As<ICacheManager>().Named<ICacheManager>("gather_cache_static").SingleInstance();
            
            // Helpers
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerApiRequest();

            // Work context
            // NOTE: Not currently implemented as I don't yet know how to obtain current user.
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerApiRequest();

            // Services
            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerApiRequest();
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerApiRequest();
            builder.RegisterType<CommentService>().As<ICommentService>().InstancePerApiRequest();
            builder.RegisterType<GeolocationService>().As<IGeolocationService>().InstancePerApiRequest();
            builder.RegisterType<LocationService>().As<ILocationService>().InstancePerApiRequest();
            builder.RegisterType<MediaService>().As<IMediaService>().InstancePerApiRequest();
            builder.RegisterType<MessageQueueService>().As<IMessageQueueService>().InstancePerApiRequest();
            builder.RegisterType<ModerationQueueService>().As<IModerationQueueService>().InstancePerApiRequest();
            builder.RegisterType<SuccessStoryService>().As<ISuccessStoryService>().InstancePerApiRequest();
            builder.RegisterType<SlugService>().As<ISlugService>().InstancePerApiRequest();
            builder.RegisterType<PageService>().As<IPageService>().InstancePerApiRequest();
            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerApiRequest();
            builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerApiRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerApiRequest();
            builder.RegisterType<TweetService>().As<ITweetService>().InstancePerApiRequest();

            builder.RegisterType<ApiAuthenticationService>().As<IApiAuthenticationService>().InstancePerApiRequest();
            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerApiRequest();

            builder.RegisterGeneric(typeof(ConfigurationProvider<>)).As(typeof(IConfigurationProvider<>));
            builder.RegisterSource(new SettingsSource());

            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerApiRequest();
        }

        public class SettingsSource : IRegistrationSource
        {
            static readonly MethodInfo _buildMethod = typeof(SettingsSource).GetMethod("BuildRegistration", BindingFlags.Static | BindingFlags.NonPublic);

            public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrations)
            {
                var ts = service as TypedService;
                if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
                {
                    var buildMethod = _buildMethod.MakeGenericMethod(ts.ServiceType);
                    yield return (IComponentRegistration)buildMethod.Invoke(null, null);
                }
            }

            static IComponentRegistration BuildRegistration<T>() where T : ISettings, new()
            {
                return RegistrationBuilder
                    .ForDelegate((c, p) => c.Resolve<IConfigurationProvider<T>>().Settings)
                    .InstancePerApiRequest()
                    .CreateRegistration();
            }

            public bool IsAdapterForIndividualComponents { get { return false; } }
        }

        public int Order
        {
            get { return 0; }
        }
    }
}