using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;
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
using Gather.Services.Comments;
using Gather.Services.Configuration;
using Gather.Services.Geolocation;
using Gather.Services.Install;
using Gather.Services.Logging;
using Gather.Services.MessageQueues;
using Gather.Services.ModerationQueues;
using Gather.Services.Security;
using Gather.Services.Settings;
using Gather.Services.Slugs;
using Gather.Services.Tasks;
using Gather.Services.Tweets;
using Gather.Services.Users;
using Gather.Web.Framework.Mvc.Routes;
using Gather.Web.Framework.UI;
using Gather.Services.Categories;
using Gather.Services.Locations;
using Gather.Services.SuccessStories;
using Gather.Services.Pages;
using Gather.Services.Projects;
using Gather.Web.Framework.UI.Breadcrumbs;
using Gather.Web.Framework.UI.Tabbing;
using Gather.Services.MediaFile;
using Gather.Services.Profanities;

namespace Gather.Web.Framework
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            // HTTP context
            builder.Register(c => HttpContext.Current != null ? (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) : (new FakeHttpContext("~/") as HttpContextBase)).As<HttpContextBase>().InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Request).As<HttpRequestBase>().InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Response).As<HttpResponseBase>().InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Server).As<HttpServerUtilityBase>().InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Session).As<HttpSessionStateBase>().InstancePerHttpRequest();

            // Helpers
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerHttpRequest();
            builder.RegisterType<LayoutPropertyHelper>().As<ILayoutPropertyHelper>().InstancePerHttpRequest();
            builder.RegisterType<BreadcrumbHelper>().As<IBreadcrumbHelper>().InstancePerHttpRequest();
            builder.RegisterType<TabHelper>().As<ITabHelper>().InstancePerHttpRequest();

            // Controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            // Data access
            var dataSettingsManager = new DataSettingsManager();
            builder.Register(x => (IEfDataProvider) new EfDataProvider()).As<IDataProvider>().InstancePerDependency();
            builder.Register(x => (IEfDataProvider) new EfDataProvider()).As<IEfDataProvider>().InstancePerDependency();
            builder.Register<IDbContext>(c => new GatherObjectContext(dataSettingsManager.LoadSettings().ConnectionString)).InstancePerHttpRequest();

            // Repo
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();

            // Cache
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("gather_cache_static").SingleInstance();

            // Work context
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerHttpRequest();

            // Services
            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerHttpRequest();
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerHttpRequest();
            builder.RegisterType<CommentService>().As<ICommentService>().InstancePerHttpRequest();
            builder.RegisterType<GeolocationService>().As<IGeolocationService>().InstancePerHttpRequest();
            builder.RegisterType<InstallService>().As<IInstallService>().InstancePerHttpRequest();
            builder.RegisterType<LocationService>().As<ILocationService>().InstancePerHttpRequest();
            builder.RegisterType<MediaService>().As<IMediaService>().InstancePerHttpRequest();
            builder.RegisterType<MessageQueueService>().As<IMessageQueueService>().InstancePerHttpRequest();
            builder.RegisterType<ModerationQueueService>().As<IModerationQueueService>().InstancePerHttpRequest();
            builder.RegisterType<PageService>().As<IPageService>().InstancePerHttpRequest();
            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerHttpRequest();
            builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerHttpRequest();
            builder.RegisterType<ProfanityService>().As<IProfanityService>().InstancePerHttpRequest();
            builder.RegisterType<SlugService>().As<ISlugService>().InstancePerHttpRequest();
            builder.RegisterType<SuccessStoryService>().As<ISuccessStoryService>().InstancePerHttpRequest();
            builder.RegisterType<TweetService>().As<ITweetService>().InstancePerHttpRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerHttpRequest();
            builder.RegisterType<ApiAuthenticationService>().As<IApiAuthenticationService>().InstancePerHttpRequest();
            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerHttpRequest();

            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerHttpRequest();
            builder.RegisterType<LogService>().As<ILogService>().InstancePerHttpRequest();

            builder.RegisterGeneric(typeof(ConfigurationProvider<>)).As(typeof(IConfigurationProvider<>));
            builder.RegisterSource(new SettingsSource());

            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerHttpRequest();
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerHttpRequest();
            builder.RegisterType<TwitterMessageSender>().As<ITwitterMessageSender>().InstancePerHttpRequest();
            builder.RegisterType<AnalyticsBuilder>().As<IAnalyticsBuilder>().InstancePerHttpRequest();

            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
        }

        public int Order
        {
            get { return 0; }
        }
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
                .InstancePerHttpRequest()
                .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents { get { return false; } }
    }
}