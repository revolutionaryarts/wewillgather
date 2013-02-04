using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using Gather.Core;
using Gather.Core.Data;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Users;
using Gather.Core.Infrastructure;
using Gather.Core.Seo;
using Gather.Data;
using Gather.Services.Install;
using Gather.Services.Settings;
using Gather.Web.Models.Install;

namespace Gather.Web.Controllers
{
    public class InstallController : Controller
    {

        #region Utilities

        private bool CheckPermissions(string path, bool checkRead, bool checkWrite, bool checkModify, bool checkDelete)
        {
            var identity = WindowsIdentity.GetCurrent();
            AuthorizationRuleCollection rules;

            if (identity == null || identity.User == null)
                return false;

            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            bool flag7 = false;
            bool flag8 = false;

            try
            {
                rules = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof (SecurityIdentifier));
            }
            catch
            {
                return true;
            }

            try
            {
                foreach (FileSystemAccessRule rule in rules.Cast<FileSystemAccessRule>().Where(rule => identity.User.Equals(rule.IdentityReference)))
                {
                    if (AccessControlType.Deny.Equals(rule.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule.FileSystemRights) == FileSystemRights.Delete)
                            flag4 = true;

                        if ((FileSystemRights.Modify & rule.FileSystemRights) == FileSystemRights.Modify)
                            flag3 = true;

                        if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                            flag = true;

                        if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                            flag2 = true;

                        continue;
                    }

                    if (AccessControlType.Allow.Equals(rule.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule.FileSystemRights) == FileSystemRights.Delete)
                            flag8 = true;

                        if ((FileSystemRights.Modify & rule.FileSystemRights) == FileSystemRights.Modify)
                            flag7 = true;

                        if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                            flag5 = true;

                        if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                            flag6 = true;
                    }
                }

                foreach (FileSystemAccessRule rule2 in identity.Groups.SelectMany(reference => rules.Cast<FileSystemAccessRule>().Where(rule2 => reference.Equals(rule2.IdentityReference))))
                {
                    if (AccessControlType.Deny.Equals(rule2.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule2.FileSystemRights) == FileSystemRights.Delete)
                            flag4 = true;

                        if ((FileSystemRights.Modify & rule2.FileSystemRights) == FileSystemRights.Modify)
                            flag3 = true;

                        if ((FileSystemRights.Read & rule2.FileSystemRights) == FileSystemRights.Read)
                            flag = true;

                        if ((FileSystemRights.Write & rule2.FileSystemRights) == FileSystemRights.Write)
                            flag2 = true;

                        continue;
                    }

                    if (AccessControlType.Allow.Equals(rule2.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule2.FileSystemRights) == FileSystemRights.Delete)
                            flag8 = true;

                        if ((FileSystemRights.Modify & rule2.FileSystemRights) == FileSystemRights.Modify)
                            flag7 = true;

                        if ((FileSystemRights.Read & rule2.FileSystemRights) == FileSystemRights.Read)
                            flag5 = true;

                        if ((FileSystemRights.Write & rule2.FileSystemRights) == FileSystemRights.Write)
                            flag6 = true;
                    }
                }

                bool flag9 = !flag4 && flag8;
                bool flag10 = !flag3 && flag7;
                bool flag11 = !flag && flag5;
                bool flag12 = !flag2 && flag6;
                bool flag13 = true;

                if (checkRead)
                    flag13 = flag11;

                if (checkWrite)
                    flag13 = flag13 && flag12;

                if (checkModify)
                    flag13 = flag13 && flag10;

                if (checkDelete)
                    flag13 = flag13 && flag9;

                return flag13;
            }
            catch
            {
            }

            return false;
        }

        private string CreateDatabase(string connectionString)
        {
            try
            {
                // Convert the connection string to builder
                var builder = new SqlConnectionStringBuilder(connectionString);

                // Get the target database name
                string databaseName = builder.InitialCatalog;

                // Alter the initial cataglog to the master
                builder.InitialCatalog = "master";
                var masterConnectionString = builder.ConnectionString;

                // Build the creation query
                string sql = string.Format("CREATE DATABASE [{0}] COLLATE SQL_Latin1_General_CP1_CI_AS", databaseName);

                // Execute the query
                using (var conn = new SqlConnection(masterConnectionString))
                {
                    conn.Open();
                    using (var command = new SqlCommand(sql, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                // Return no errors
                return null;
            }
            catch (Exception ex)
            {
                // Return the error that occurred
                return string.Format("An error occured when creating database: {0}", ex.Message);
            }
        }

        private bool DatabaseExists(string connectionString)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool DatabaseConnectionValid(string connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString) {InitialCatalog = "master"};
                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateConnectionString(bool trusted, string serverName, string databaseName, string userName, string password)
        {
            var sql = new SqlConnectionStringBuilder
                          {
                              DataSource = serverName,
                              InitialCatalog = databaseName,
                              IntegratedSecurity = trusted,
                              MultipleActiveResultSets = true,
                              PersistSecurityInfo = false
                          };

            if (!trusted)
            {
                sql.UserID = userName;
                sql.Password = password;
            }

            return sql.ConnectionString;
        }

        private StepOneModel PrepareStepOneModel(StepOneModel model = null)
        {
            if (model == null)
            {
                model = new StepOneModel();
            }

            model.AvailableAuthenticationMethods.Add(new SelectListItem {Text = "SQL Server Authentication", Value = "10"});
            model.AvailableAuthenticationMethods.Add(new SelectListItem {Text = "Windows Authentication", Value = "20"});

            return model;
        }

        private StepFourModel PrepareStepFourModel(StepFourModel model = null, User siteOwner = null)
        {
            if (model == null)
            {
                model = new StepFourModel
                            {
                                MailHost = "localhost",
                                MailPort = 21
                            };
            }

            if (siteOwner != null)
            {
                model.AuthenticationMethod = siteOwner.PrimaryAuthMethod.ToString();
                model.DisplayName = siteOwner.DisplayName;
                model.Email = siteOwner.Email;
                model.UserName = siteOwner.UserName;
            }

            return model;
        }

        #endregion

        #region Actions

        public ActionResult StepOne()
        {
            if (DataSettingsHelper.SiteIsInstalled)
                return RedirectToAction("index", "home");

            if (DataSettingsHelper.DatabaseIsInstalled)
                return RedirectToAction("steptwo", "install");

            var model = PrepareStepOneModel();

            var manager = new DataSettingsManager();
            var settings = manager.LoadSettings();
            if (settings != null)
            {
                var builder = new SqlConnectionStringBuilder(settings.ConnectionString);
                model.DatabaseName = builder.InitialCatalog;
                model.DatabasePassword = builder.Password;
                model.DatabaseServerName = builder.DataSource;
                model.DatabaseUsername = builder.UserID;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult StepOne(StepOneModel model)
        {
            var manager = new DataSettingsManager();
            model = PrepareStepOneModel(model);

            // Make sure if SQL Server authenticaton is selected, we have been given a username/password
            if (model.DatabaseAuthenticationMethod == 10)
            {
                if (string.IsNullOrEmpty(model.DatabaseUsername))
                    ModelState.AddModelError("DatabaseUsername", "Please enter your SQL username.");

                if (string.IsNullOrEmpty(model.DatabasePassword))
                    ModelState.AddModelError("DatabasePassword", "Please enter your SQL password.");
            }

            // Make sure we have the required folder permissions
            string root = Server.MapPath("~/");
            var directories = new List<string>
            {
                Path.Combine(root, "robots.txt"),
                Path.Combine(root, "App_Data"),
                Path.Combine(root, "Uploads"),
                Path.Combine(root, "Uploads\\Media"),
                Path.Combine(root, "Uploads\\Profile")
            };

            // Check each directory, flagging a modelstate error if the permissions aren't correct
            foreach (var directory in directories.Where(directory => !CheckPermissions(directory, false, true, true, true)))
                ModelState.AddModelError("", string.Format("The '{0}' account is not granted with Modify permission on folder '{1}'. Please configure these permissions.", WindowsIdentity.GetCurrent().Name, directory));

            // Ensure the rest of the form is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Build the connection string
                    string connectionString = GenerateConnectionString(model.DatabaseAuthenticationMethod == 20, model.DatabaseServerName, model.DatabaseName, model.DatabaseUsername, model.DatabasePassword);

                    // If the database doesn't exist, create it
                    if (!DatabaseExists(connectionString))
                    {
                        string databaseCreationError = CreateDatabase(connectionString);
                        if (!string.IsNullOrEmpty(databaseCreationError))
                            throw new Exception(databaseCreationError);
                        Thread.Sleep(3000);
                    }

                    // Create settings and save
                    var settings = new DataSettings {ConnectionString = connectionString};
                    manager.SaveSettings(settings);

                    // Initialise the database
                    var dataProviderInstance = new EfDataProvider();
                    dataProviderInstance.InitDatabase();

                    // Install the core data
                    var installService = EngineContext.Current.Resolve<IInstallService>();
                    bool coreDataInstalled = installService.InstallCoreData();

                    // Make sure the core data installed correctly before running on ahead
                    if (!coreDataInstalled)
                        ModelState.AddModelError("", "An error occurred during installation, please try again.");
                    else
                        return RedirectToAction("steptwo", "install");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Installation failed: " + ex);
                }
            }

            return View(model);
        }

        public ActionResult StepTwo()
        {
            if (DataSettingsHelper.SiteIsInstalled)
                return RedirectToAction("index", "home");

            if (!DataSettingsHelper.DatabaseIsInstalled)
                return RedirectToRoute("Install");

            var model = new StepTwoModel();

            var siteSettings = EngineContext.Current.Resolve<SiteSettings>();
            if (siteSettings != null)
            {
                model.FacebookAppId = siteSettings.FacebookAppId;
                model.FacebookAppSecret = siteSettings.FacebookAppSecret;
                model.TwitterConsumerKey = siteSettings.TwitterConsumerKey;
                model.TwitterConsumerSecret = siteSettings.TwitterConsumerSecret;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult StepTwo(StepTwoModel model)
        {
            if (DataSettingsHelper.SiteIsInstalled)
                return RedirectToAction("index", "home");

            if (!DataSettingsHelper.DatabaseIsInstalled)
                return RedirectToRoute("Install");

            // Ensure the rest of the form is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing site settings
                    var settingService = EngineContext.Current.Resolve<ISettingService>();
                    var siteSettings = EngineContext.Current.Resolve<SiteSettings>();

                    // Update the social integration settings
                    siteSettings.FacebookAppId = model.FacebookAppId;
                    siteSettings.FacebookAppSecret = model.FacebookAppSecret;
                    siteSettings.TwitterConsumerKey = model.TwitterConsumerKey;
                    siteSettings.TwitterConsumerSecret = model.TwitterConsumerSecret;

                    // Update the settings
                    settingService.SaveSetting(siteSettings);
                    return RedirectToAction("stepthree", "install");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Installation failed: " + ex);
                }
            }

            return View(model);
        }

        public ActionResult StepThree()
        {
            if (DataSettingsHelper.SiteIsInstalled)
                return RedirectToAction("index", "home");

            if (!DataSettingsHelper.DatabaseIsInstalled)
                return RedirectToRoute("Install");

            return View();
        }

        public ActionResult StepFour()
        {
            if (DataSettingsHelper.SiteIsInstalled)
                return RedirectToAction("index", "home");

            if (!DataSettingsHelper.DatabaseIsInstalled)
                return RedirectToRoute("Install");

            var manager = new DataSettingsManager();
            var settings = manager.LoadSettings();

            if (settings.SiteOwner == null)
                return RedirectToRoute("Install");

            var model = PrepareStepFourModel(siteOwner: settings.SiteOwner);
            return View(model);
        }

        [HttpPost]
        public ActionResult StepFour(StepFourModel model)
        {
            if (DataSettingsHelper.SiteIsInstalled)
                return RedirectToAction("index", "home");

            if (!DataSettingsHelper.DatabaseIsInstalled)
                return RedirectToRoute("Install");

            // Load the local settings
            var manager = new DataSettingsManager();
            var settings = manager.LoadSettings();

            // Ensure the form is valid
            if (ModelState.IsValid)
            {
                var siteOwner = settings.SiteOwner;
                siteOwner.DisplayName = model.DisplayName;
                siteOwner.Email = model.Email;
                siteOwner.UserName = SeoExtensions.GetSeoName(model.UserName);

                // Install the rest of the site data
                var installService = EngineContext.Current.Resolve<IInstallService>();
                installService.InstallUserData(siteOwner, model.MailFromDisplayName, model.MailFromEmail, model.MailHost, model.MailPort, model.MailUsername, model.MailPassword, model.TwitterAccessToken, model.TwitterAccessTokenSecret);

                // Clear our the site owner data from the settings file
                settings.InstallComplete = true;
                settings.SiteOwner = null;
                manager.SaveSettings(settings);

                // Restart the application
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                webHelper.RestartAppDomain();

                // Redirect home
                return RedirectToAction("index", "home");
            }

            model = PrepareStepFourModel(model);
            return View(model);
        }

        #endregion

    }
}