using System;
using System.Linq;
using System.Web.Mvc;
using Gather.Core;
using Gather.Core.Domain.Api;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Users;
using Gather.Services.ApiAuthentications;
using Gather.Services.Authentication;
using Gather.Services.MessageQueues;
using Gather.Services.Projects;
using Gather.Services.Users;
using Gather.Web.Extensions;
using Gather.Web.Framework.Controllers;
using Gather.Web.Models.Api;
using Gather.Web.Models.User;

namespace Gather.Web.Controllers
{
    public class UserController : BaseController
    {

        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly IApiAuthenticationService _apiAuthenticationService;
        private readonly IMessageQueueService _messageQueueService;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        private const string KEEP_EDIT_PANEL_OPEN = "KeepEditPanelOpen";

        #endregion

        #region Constructors

        public UserController(IAuthenticationService authenticationService, IMessageQueueService messageQueueService, IProjectService projectService, IUserService userService, IWebHelper webHelper, IWorkContext workContext, IApiAuthenticationService apiAuthenticationService)
        {
            _authenticationService = authenticationService;
            _apiAuthenticationService = apiAuthenticationService;
            _messageQueueService = messageQueueService;
            _projectService = projectService;
            _userService = userService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        private UserProfileModel PrepareUserProfileModel(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            AddHomeBreadcrumb();
            AddBreadcrumb("Profile for " + user.DisplayName, Url.RouteUrl("UserProfile", new { userName = user.UserName }));

            var model = new UserProfileModel
            {
                FinishedProjects = _projectService.GetFinishedProjectsByUserId(user.Id).Select(x => x.ToModel()).ToList(),
                IsOwnProfile = (_workContext.CurrentUser != null && user.Id == _workContext.CurrentUser.Id),
                MetaTitle = string.Format("User profile for {0} | Volunteer on #WeWillGather", user.DisplayName),
                OrganisedProjects = _projectService.GetOrganisedProjectsByUserId(user.Id).Select(x => x.ToModel()).ToList(),
                UpcomingProjects = _projectService.GetUpcomingProjectsByUserId(user.Id).Select(x => x.ToModel()).ToList(),
                User = user.ToModel()
            };

            if (model.IsOwnProfile)
            {
                model.User.AvailableDisclosureLevels = _webHelper.GetAllEnumListItems<DisclosureLevel>();
            }
            else
            {
                if (_workContext.CurrentUser != null)
                {
                    if (model.OrganisedProjects.Any(x => x.Volunteers.Any(y => y.Id == _workContext.CurrentUser.Id) || x.Owners.Any(y => y.Id == _workContext.CurrentUser.Id)))
                        model.VisitorIsVolunteer = true;
                }
            }

            return model;
        }

        private UserApiModel PrepareApiProfileModel(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            AddHomeBreadcrumb();
            AddBreadcrumb("Profile for " + user.DisplayName, Url.RouteUrl("UserProfile", new { userName = user.UserName }));
            AddBreadcrumb("Api Details", Url.RouteUrl("UserProfileApi"));

            var model = new UserApiModel
            {
                CurrentApiAuthentication = _apiAuthenticationService.GetApiAuthenticationByUser(user.Id).Select(x => x.ToModel()).ToList()
            };

            return model;
        }

        private ApiAuthenticationModel PrepareApiEditProfileModel(User user, int id)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            AddHomeBreadcrumb();
            AddBreadcrumb("Profile for " + user.DisplayName, Url.RouteUrl("UserProfile", new { userName = user.UserName }));
            AddBreadcrumb("Api Details", Url.RouteUrl("UserProfileApi"));
            AddBreadcrumb("Edit Api Details", Url.RouteUrl("UserProfileApiEdit", new { id }));
            
            var model = _apiAuthenticationService.GetApiAuthenticationByUserIdandId(user.Id, id).ToModel();
            
            return model;
        }

        private ActionResult DeleteAccount()
        {
            var user = _workContext.CurrentUser;
            if (user == null || !user.Active)
                return RedirectToAction("Index", "Home");

            _userService.DeleteUser(user);
            _authenticationService.SignOut();

            SuccessNotification("Your profile has been successfully deleted from the site.");
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Methods

        public ActionResult Delete()
        {
            if (_workContext.CurrentUser == null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost, ActionName("Delete")]
        [FormValueRequired("cancelDelete")]
        public ActionResult CancelDelete()
        {
            if (_workContext.CurrentUser == null || !_workContext.CurrentUser.Active)
                return RedirectToAction("Index", "Home");

            return RedirectToRoute("UserProfile", new { userName = _workContext.CurrentUser.UserName });
        }

        [HttpPost, ActionName("Delete")]
        [FormValueRequired("confirmDelete")]
        public ActionResult ConfirmDelete()
        {
            return DeleteAccount();
        }

        [Authorize]
        public ActionResult ProfileApi()
        {
            var user = _userService.GetUserById(_workContext.CurrentUser.Id);
            if (user == null || !user.Active)
                return RedirectToAction("error404", "error");

            var model = PrepareApiProfileModel(user);
            
            return View(model);
        }

        [Authorize]
        [HttpPost, ActionName("ProfileApi")]
        [FormValueRequired("GenerateAPIkey")]
        public ActionResult ProfileApiAdd(UserApiModel form)
        {
            var user = _userService.GetUserById(_workContext.CurrentUser.Id);
            if (user == null || !user.Active)
                return RedirectToAction("error404", "error");

            var model = PrepareApiProfileModel(user);

            if (form.ApiAuthentication.WebsiteAddress != null) { model.ApiAuthentication.WebsiteAddress = form.ApiAuthentication.WebsiteAddress.ToLower().Replace("http://", "").TrimEnd('/'); }
            if (form.ApiAuthentication.NameOfApplication != null) { model.ApiAuthentication.NameOfApplication = form.ApiAuthentication.NameOfApplication; }
            if (form.ApiAuthentication.Description != null) { model.ApiAuthentication.Description = form.ApiAuthentication.Description; }

            if (ModelState.IsValid)
            {
                ApiAuthentication api = model.ApiAuthentication.ToEntity();
                api.ApiUser = user;
                _apiAuthenticationService.InsertApiUsage(api);
                model.ShowToken = true;
                model.ApiAuthentication.AccessToken = api.AccessToken;
            }

            model.CurrentApiAuthentication = _apiAuthenticationService.GetApiAuthenticationByUser(user.Id).Select(x => x.ToModel()).ToList();

            return View(model);
        }

        [Authorize]
        [HttpPost, ActionName("ProfileApi")]
        [FormValueRequired("Deletekey")]
        public ActionResult ProfileApiDelete(UserApiModel form)
        {
            var user = _userService.GetUserById(_workContext.CurrentUser.Id);
            if (user == null || !user.Active)
                return RedirectToAction("error404", "error");

            int apiId;
            int.TryParse(Request.Form["api.Id"], out apiId);

            if (apiId > 0)
            {                
                var api = _apiAuthenticationService.GetApiAuthenticationByUserIdandId(user.Id, apiId);

                if (api != null)
                {
                    _apiAuthenticationService.DeleteApiUsage(api);
                }
            }

            var model = PrepareApiProfileModel(user);

            return View(model);
        }

        [Authorize]
        public ActionResult ProfileApiEdit(int id)
        {
            var user = _userService.GetUserById(_workContext.CurrentUser.Id);
            if (user == null || !user.Active)
                return RedirectToAction("error404", "error");

            var model = PrepareApiEditProfileModel(user, id);

            if (model == null)
            {
                ErrorNotification("You do not have permissions to access this page");
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [Authorize]
        [HttpPost, ActionName("ProfileApiEdit")]
        public ActionResult ProfileApiEdit(ApiAuthenticationModel form)
        {
            var user = _userService.GetUserById(_workContext.CurrentUser.Id);
            if (user == null || !user.Active)
                return RedirectToAction("error404", "error");

            AddHomeBreadcrumb();
            AddBreadcrumb("Profile for " + user.DisplayName, Url.RouteUrl("UserProfile", new { userName = user.UserName }));
            AddBreadcrumb("Api Details", Url.RouteUrl("UserProfileApi"));
            AddBreadcrumb("Edit Api Details", Url.RouteUrl("UserProfileApiEdit", new { form.Id }));

            var api = _apiAuthenticationService.GetApiAuthenticationByUserIdandId(user.Id, form.Id);
            var model = api.ToModel();

            if (model == null)
            {
                ErrorNotification("You do not have permissions to access this page");
                return RedirectToAction("index", "home");
            }

            if (!string.IsNullOrEmpty(form.WebsiteAddress)) model.WebsiteAddress = form.WebsiteAddress.ToLower().Replace("http://", "").TrimEnd('/');
            if (!string.IsNullOrEmpty(form.NameOfApplication)) model.NameOfApplication = form.NameOfApplication;
            if (!string.IsNullOrEmpty(form.Description)) model.Description = form.Description;

            if (ModelState.IsValid)
            {                
                api.ApiUser = user;
                api.WebsiteAddress = model.WebsiteAddress;
                api.NameOfApplication = model.NameOfApplication;
                api.Description = model.Description;
                _apiAuthenticationService.UpdateApiUsage(api);
                model.ShowToken = true;
                model.AccessToken = api.AccessToken;
            }

            return View(model);
        }

        public new ActionResult Profile(string userName)
        {
            var user = _userService.GetUserByUserName(userName);
            if (user == null || !user.Active)
                return RedirectToAction("error404", "error");

            var model = PrepareUserProfileModel(user);

            if(TempData[KEEP_EDIT_PANEL_OPEN] != null)
                model.ShowEditProfile = true;

            return View(model);
        }

        [HttpPost, ActionName("Profile")]
        [FormValueRequired("cancelEditProfile")]
        public ActionResult CancelEditProfile(string userName)
        {
            var user = _userService.GetUserByUserName(userName);
            if (user == null || !user.Active)
                return RedirectToAction("error404", "error");

            var model = PrepareUserProfileModel(user);
            model.ShowEditProfile = false;

            return View(model);
        }

        [HttpPost, ActionName("Profile")]
        [FormValueRequired("deleteProfile")]
        public ActionResult DeleteProfile()
        {
            return DeleteAccount();
        }

        [HttpPost, ActionName("Profile")]
        [FormValueRequired("editProfile")]
        public ActionResult EditProfile(string userName)
        {
            var user = _userService.GetUserByUserName(userName);
            if (user == null || !user.Active)
                return RedirectToAction("error404", "error");

            var model = PrepareUserProfileModel(user);
            model.ShowEditProfile = true;

            return View(model);
        }

        [HttpPost, ActionName("Profile")]
        [FormValueRequired("unlinkProfile")]
        public ActionResult UnlinkProfile(string userName, string profileType)
        {
            var user = _userService.GetUserByUserName(userName);
            if (user == null || !user.Active)
                return RedirectToRoute("error404", "error");

            switch (profileType)
            {
                case "facebook":

                    // Make sure the user also has a Twitter profile linked
                    if (user.TwitterProfile != null)
                    {
                        user.FacebookDisplayName = null;
                        user.FacebookProfile = null;

                        if (user.PrimaryAuthMethod == AuthenticationMethod.Facebook)
                            user.PrimaryAuthMethod = AuthenticationMethod.Twitter;

                        _userService.UpdateUser(user);
                        SuccessNotification("Your Facebook profile has been successfully unlinked.");
                    }
                    else
                    {
                        ErrorNotification("You cannot unlink a Facebook profile if the user doesn't have a Twitter profile also linked.");
                    }

                    break;
                case "twitter":

                    // Make sure the user also has a Facebook profile linked
                    if (user.FacebookProfile != null)
                    {
                        user.TwitterDisplayName = null;
                        user.TwitterProfile = null;

                        if (user.PrimaryAuthMethod == AuthenticationMethod.Twitter)
                            user.PrimaryAuthMethod = AuthenticationMethod.Facebook;
                        
                        _userService.UpdateUser(user);
                        SuccessNotification("Your Twitter profile has been successfully unlinked.");
                    }
                    else
                    {
                        ErrorNotification("You cannot unlink a Twitter profile if the user doesn't have a Facebook profile also linked.");
                    }

                    break;
                default:

                    ErrorNotification("An error occurred unlinking the profile, please try again.");

                    break;
            }

            TempData.Add(KEEP_EDIT_PANEL_OPEN, true);
            return RedirectToRoute("UserProfile", new { userName });
        }

        [HttpPost, ActionName("Profile")]
        [FormValueRequired("updateProfile")]
        public ActionResult UpdateProfile(string userName, UserProfileModel model)
        {
            var user = _userService.GetUserByUserName(userName);
            if (user == null || !user.Active)
                return RedirectToAction("error404", "error");

            bool updateUserName = model.User.UserName != user.UserName;

            user.DisplayName = model.User.DisplayName;
            user.Email = model.User.Email;
            user.Telephone = model.User.Telephone;
            user.UserName = model.User.UserName;
            user.Website = model.User.Website;

            if (Enum.IsDefined(typeof(DisclosureLevel), model.User.EmailDisclosureId))
                user.EmailDisclosureId = model.User.EmailDisclosureId;

            if (Enum.IsDefined(typeof(DisclosureLevel), model.User.TelephoneDisclosureId))
                user.TelephoneDisclosureId = model.User.TelephoneDisclosureId;

            if (Enum.IsDefined(typeof(DisclosureLevel), model.User.WebsiteDisclosureId))
                user.WebsiteDisclosureId = model.User.WebsiteDisclosureId;

            model = PrepareUserProfileModel(user);

            if (ModelState.IsValid)
            {
                try
                {
                    _userService.UpdateUser(user);
                    SuccessNotification("Your profile details have been updated successfully.");

                    if (updateUserName)
                        return RedirectToRoute("UserProfile", new { userName = user.UserName });
                }
                catch (Exception)
                {
                    ErrorNotification("An error occurred updating your profile details, please try again.");
                }
            }
            else
            {
                model.ShowEditProfile = true;
            }

            return View(model);
        }

        public ActionResult SwitchPrimaryAccount()
        {
            var user = _workContext.CurrentUser;
            if (user == null)
                return RedirectToAction("Index", "Home");

            if (user.FacebookProfile != null && user.TwitterProfile != null)
            {
                user.PrimaryAuthMethod = (user.PrimaryAuthMethod == AuthenticationMethod.Facebook ? AuthenticationMethod.Twitter : AuthenticationMethod.Facebook);
                _userService.UpdateUser(user);
                SuccessNotification("Your primary social media account has been successfully changed.");
                TempData.Add(KEEP_EDIT_PANEL_OPEN, true);
            }
            else
            {
                ErrorNotification("You cannot change your primary social media account because you only have 1 account linked to your profile.");
            }

            return RedirectToRoute("UserProfile", new { userName = user.UserName });
        }

        public ActionResult UnlinkProfile(string profileType)
        {
            var user = _workContext.CurrentUser;
            if (user == null)
                return RedirectToAction("Index", "Home");

            switch (profileType)
            {
                case "facebook":

                    // Make sure the user also has a Twitter profile linked
                    if (user.TwitterProfile != null)
                    {
                        user.FacebookDisplayName = null;
                        user.FacebookProfile = null;

                        if (user.PrimaryAuthMethod == AuthenticationMethod.Facebook)
                            user.PrimaryAuthMethod = AuthenticationMethod.Twitter;

                        _userService.UpdateUser(user);
                        SuccessNotification("Your Facebook profile has been successfully unlinked.");
                        TempData.Add(KEEP_EDIT_PANEL_OPEN, true);
                    }
                    else
                    {
                        ErrorNotification("You cannot unlink a Facebook profile if the user doesn't have a Twitter profile also linked.");
                    }

                    break;
                case "twitter":

                    // Make sure the user also has a Facebook profile linked
                    if (user.FacebookProfile != null)
                    {
                        user.TwitterDisplayName = null;
                        user.TwitterProfile = null;

                        if (user.PrimaryAuthMethod == AuthenticationMethod.Twitter)
                            user.PrimaryAuthMethod = AuthenticationMethod.Facebook;

                        _userService.UpdateUser(user);
                        SuccessNotification("Your Twitter profile has been successfully unlinked.");
                        TempData.Add(KEEP_EDIT_PANEL_OPEN, true);
                    }
                    else
                    {
                        ErrorNotification("You cannot unlink a Twitter profile if the user doesn't have a Facebook profile also linked.");
                    }

                    break;
            }

            return RedirectToRoute("UserProfile", new { userName = user.UserName });
        }

        [Authorize]
        public ActionResult Message(int id)
        {
            var message = _messageQueueService.GetMessageQueueById(id);

            if (message == null)
            {
                ErrorNotification("We have been unable retrieve the message.");
                return RedirectToAction("Index", "Home");
            }

            if (message.User.Id != _workContext.CurrentUser.Id)
            {
                ErrorNotification("You do not have permission to read this message.");
                return RedirectToAction("Index", "Home");
            }

            return View(message.ToModel());
        }

        #endregion

    }
}