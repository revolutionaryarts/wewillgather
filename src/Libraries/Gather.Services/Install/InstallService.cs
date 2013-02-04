using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Gather.Core;
using Gather.Core.Configuration;
using Gather.Core.Data;
using Gather.Core.Domain.Categories;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.MediaFile;
using Gather.Core.Domain.Pages;
using Gather.Core.Domain.Profanity;
using Gather.Core.Domain.SuccessStories;
using Gather.Core.Domain.Tasks;
using Gather.Core.Domain.Users;
using Gather.Core.Infrastructure;
using Gather.Data;
using Gather.Services.Authentication;
using Gather.Services.Security;
using Gather.Services.Users;
using Newtonsoft.Json;

namespace Gather.Services.Install
{
    public class InstallService : IInstallService
    {

        #region Variables
        
        private readonly IAuthenticationService _authService;
        private readonly IDbContext _dbContext;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Media> _mediaRepository; 
        private readonly IRepository<Page> _pageRepository;
        private readonly IRepository<Profanity> _profanityRepository;
        private readonly IRepository<SuccessStory> _successStoryRepository; 
        private readonly IRepository<ScheduleTask> _taskRepository;
        private readonly IRepository<User> _userRepository; 
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IUserService _userService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Constructors

        public InstallService(IAuthenticationService authService, IDbContext dbContext, IRepository<Category> categoryRepository, IRepository<Media> mediaRepository, IRepository<Page> pageRepository, IRepository<Profanity> profanityRepository, IRepository<SuccessStory> successStoryRepository, IRepository<ScheduleTask> taskRepository, IRepository<User> userRepository, IRepository<UserRole> userRoleRespository, IUserService userService, IWebHelper webHelper)
        {
            _authService = authService;
            _dbContext = dbContext;
            _categoryRepository = categoryRepository;
            _mediaRepository = mediaRepository;
            _pageRepository = pageRepository;
            _profanityRepository = profanityRepository;
            _successStoryRepository = successStoryRepository;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRespository;
            _userService = userService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        public bool InstallCoreData()
        {
            // Attempt to install the site locations, this is key to the site 
            // functioning correctly, if this doesn't work, throw it back
            bool outcome = InstallLocations();

            // If the locations were installed correctly, 
            // we're good to go with installing everything else
            if (outcome)
            {
                InstallCoreSettings();
                InstallCategories();
                InstallProfanity();
                InstallScheduledTasks();
                CreateRobots();
            }

            return outcome;
        }

        public void InstallUserData(User siteOwner, string mailFromDisplayName, string mailFromEmail, string mailHost, int mailPort, string mailUsername, string mailPassword, string twitterAccessToken, string twitterAccessTokenSecret)
        {
            InstallUsers(siteOwner);
            InstallPermissions();
            InstallPages();
            InstallSuccessStories();
            InstallUserSettings(mailFromDisplayName, mailFromEmail, mailHost, mailPort, mailUsername, mailPassword, twitterAccessToken, twitterAccessTokenSecret);
        }

        #endregion

        #region Utilities

        private void CreateRobots()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/robots.txt");
            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Disallow: ");
            sb.AppendLine(Environment.NewLine + "Sitemap: " + _webHelper.GetSiteLocation(false) + "site-map.xml");
            File.WriteAllText(filePath, sb.ToString());
        }

        private void InstallCategories()
        {
            var categories = new List<string>
            {
                "Arts & Culture",
                "Children & Youth Education",
                "Cleaning",
                "Decorating",
                "Empty Shops",
                "Environment",
                "Gardening",
                "Fixing",
                "Fundraising",
                "Making",
                "Meeting",
                "Repairing",
                "Technology"
            };

            foreach (var name in categories)
            {
                var category = new Category
                {
                    Active = true,
                    CreatedDate = DateTime.Now,
                    Deleted = false,
                    LastModifiedDate = DateTime.Now,
                    Name = name
                };

                _categoryRepository.Insert(category);
            }
        }

        private bool InstallLocations()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/App_Data/locations.txt");

            if (File.Exists(filePath))
            {
                var locations = File.ReadAllText(filePath);
                var query = _dbContext.SqlQuery<bool>("EXEC [Location_Import] @p0", locations);
                var outcome = query.ToList();
                return outcome.FirstOrDefault();
            }

            return false;
        }

        private void InstallPages()
        {
            var pages = new List<Page>
            {
                new Page {
                    Priority = 0.5m,
                    Title = "About Us"
                },
                new Page {
                    Priority = 0.7m,
                    Title = "Be Safe"
                },
                new Page {
                    Priority = 0.5m,
                    Title = "Cookie Policy"
                },
                new Page {
                    Priority = 0.6m,
                    Title = "Developers"
                },
                new Page {
                    Priority = 0.7m,
                    Title = "How it Works"
                },
                new Page {
                    Priority = 0.7m,
                    Title = "Our Supporters"
                },
                new Page {
                    Priority = 0.5m,
                    Title = "Privacy Policy"
                },
                new Page {
                    Priority = 0.5m,
                    Title = "Terms & Conditions"
                }
            };

            foreach (var page in pages)
            {
                page.Active = true;
                page.Content += string.Format("Welcome to the {0} page. This content can be changed from the admin area.", page.Title);
                page.CreatedBy = 1;
                page.CreatedDate = DateTime.Now;
                page.Deleted = false;
                page.IsSystemPage = true;
                page.LastModifiedDate = DateTime.Now;
                _pageRepository.Insert(page);
            }
        }

        private void InstallPermissions()
        {
            var permissions = EngineContext.Current.Resolve<IPermissionService>();
            permissions.InstallPermissions();
        }

        private void InstallProfanity()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/App_Data/profanity.txt");
            if (File.Exists(filePath))
            {
                var raw = File.ReadAllText(filePath);
                var profanity = JsonConvert.DeserializeObject<List<string>>(raw);
                if (profanity != null)
                {
                    foreach (var word in profanity)
                    {
                        _profanityRepository.Insert(new Profanity
                        {
                            Active = true,
                            Deleted = false,
                            Word = word
                        });
                    }
                }
            }
        }

        private void InstallScheduledTasks()
        {
            var tasks = new List<ScheduleTask>
            {
                new ScheduleTask
                {
                    Name = "Keep alive",
                    Seconds = 600,
                    Type = "Gather.Services.Common.KeepAliveTask, Gather.Services",
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Twitter Monitor",
                    Seconds = 30,
                    Type = "Gather.Services.Projects.Tools.TwitterMonitorTask, Gather.Services",
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Send Message",
                    Seconds = 300,
                    Type = "Gather.Services.MessageQueues.Tools.MessageQueueTask, Gather.Services",
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Send Tweets",
                    Seconds = 60,
                    Type = "Gather.Services.MessageQueues.Tools.TwitterQueueTask, Gather.Services",
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Project Monitor",
                    Seconds = 60,
                    Type = "Gather.Services.Projects.Tools.ProjectMonitorTask, Gather.Services",
                    StopOnError = false,
                }
            };

            foreach (var task in tasks)
            {
                task.Enabled = false;
                _taskRepository.Insert(task);
            }
        }

        private void InstallCoreSettings()
        {
            EngineContext.Current.Resolve<IConfigurationProvider<CoreSettings>>().SaveSettings(new CoreSettings
            {
                AdminGridPageSize = 30,
                Domain = _webHelper.GetSiteLocation(false),
                FacebookStateSecret = _webHelper.RandomString(10),
                LastTweetId = "0",
                TwitterStateSecret = _webHelper.RandomString(10)
            });

            EngineContext.Current.Resolve<IConfigurationProvider<SiteSettings>>().SaveSettings(new SiteSettings
            {
                CommentModerationRequestLimit = "10",
                FacebookAppId = "",
                FacebookAppSecret = "",
                GoogleAnalyticsEnabled = "false",
                GoogleAnalyticsUACode = "UA-000000-00",
                HomePageSuccessStoryCount = "3",
                ProjectListingPageSize = "10",
                SuccessStoryListingPageSize = "6",
                SuccessStoryRssSize = "25",
                TwitterConsumerKey = "",
                TwitterConsumerSecret = "",
                TwitterHashTag = "#wewillgather",
                TwitterQuery = "#wewillgather"
            });
        }

        private void InstallUserSettings(string mailFromDisplayName, string mailFromEmail, string mailHost, int mailPort, string mailUsername, string mailPassword, string twitterAccessToken, string twitterAccessTokenSecret)
        {
            EngineContext.Current.Resolve<IConfigurationProvider<OwnerSettings>>().SaveSettings(new OwnerSettings
            {
                MailDefaultCredentials = false,
                MailEnableSSL = false,
                MailFromDisplayName = mailFromDisplayName,
                MailFromEmail = mailFromEmail,
                MailHost = mailHost,
                MailPassword = mailPassword,
                MailPort = mailPort.ToString(),
                MailUsername = mailUsername,
                TwitterAccessToken = twitterAccessToken,
                TwitterAccessTokenSecret = twitterAccessTokenSecret
            });
        }

        private void InstallSuccessStories()
        {
            var story = new SuccessStory
            {
                Active = true,
                Article = "Congratulations, you've successfully installed #wewillgather. You can delete this post and add your own new ones via the site admin area.",
                Author = _userService.GetSiteOwner(),
                CreatedBy = 1,
                CreatedDate = DateTime.Now,
                Deleted = false,
                LastModifiedDate = DateTime.Now,
                Title = "You've installed #wewillgather!"                
            };

            _successStoryRepository.Insert(story);

            var storyImage = new Media
            {
                EntityId = story.Id,
                EntityType = EntityType.SuccessStory,
                FileName = "install-post.jpg",
                FileType = FileType.Image,
                UploadedById = 1,
                UploadedDate = DateTime.Now
            };

            _mediaRepository.Insert(storyImage);
        }

        public void InstallUsers(User siteOwner)
        {
            var siteOwnerRole = new UserRole
            {
                Name = "Site Owner",
                SystemName = SystemUserRoleNames.SiteOwner
            };

            var administratorRole = new UserRole
            {
                Name = "Administrators",
                SystemName = SystemUserRoleNames.Administrators
            };

            var moderatorRole = new UserRole
            {
                Name = "Moderators",
                SystemName = SystemUserRoleNames.Moderators
            };

            var memberRole = new UserRole
            {
                Name = "Members",
                SystemName = SystemUserRoleNames.Members
            };

            var roles = new List<UserRole>
            {
                siteOwnerRole,
                administratorRole,
                moderatorRole,
                memberRole
            };

            // Insert each roles
            foreach (var role in roles)
            {
                role.Active = true;
                role.CreatedDate = DateTime.Now;
                role.IsSystemRole = true;
                _userRoleRepository.Insert(role);
            }

            // Create the site owner
            siteOwner.Active = true;
            siteOwner.CreatedDate = DateTime.Now;
            siteOwner.LastLoginDate = DateTime.Now;
            siteOwner.UserRoles.Add(siteOwnerRole);
            _userRepository.Insert(siteOwner);

            // Login the user
            _authService.SignIn(siteOwner, true);
        }

        #endregion

    }
}