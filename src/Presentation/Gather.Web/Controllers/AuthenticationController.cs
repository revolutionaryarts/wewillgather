using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Facebook;
using Gather.Core;
using Gather.Core.Data;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Users;
using Gather.Core.Infrastructure;
using Gather.Services.Authentication;
using Gather.Services.Logging;
using Gather.Services.Users;
using Gather.Web.Models.Authentication;
using Twitterizer;

namespace Gather.Web.Controllers
{
    public class AuthenticationController : BaseController
    {

        #region Variables

        private readonly CoreSettings _coreSettings;
        private readonly ILogService _logService;
        private readonly SiteSettings _siteSettings;
        private readonly IWebHelper _webHelper;

        private const string FACEBOOK_SESSION_COOKIE = ".GATHERAUTH";

        #endregion

        #region Constructors

        public AuthenticationController(CoreSettings coreSettings, ILogService logService, SiteSettings siteSettings, IWebHelper webHelper)
        {
            _coreSettings = coreSettings;
            _logService = logService;
            _siteSettings = siteSettings;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        public ActionResult Error(string errorDescription, string errorTitle = null, string display = null)
        {
            var model = new AuthenticationModel
            {
                ErrorDescription = errorDescription,
                ErrorTitle = errorTitle
            };

            if (!string.IsNullOrEmpty(display) && display == "popup")
                model.Layout = "~/Views/Shared/_Popup.cshtml";

            return View("Error", model);
        }

        public ActionResult Facebook(string display, string result, string state, string error, string code, string returnUrl)
        {
            // Check if we have a 'state' parameter
            if (!string.IsNullOrWhiteSpace(state))
            {
                // Build the state string we're expecting
                string tempState = BuildFacebookResponseState();

                // Validate the state returned from FB
                if (state == tempState)
                {
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        // Check if the authorization has been denied
                        string errorReason = Request.QueryString["error_reason"];

                        // Return a denied message
                        if (errorReason == "user_denied")
                            return Error("Sorry, but we cannot continue without your authorization.", "Authorization denied");

                        // Display a generic error message
                        return Error("An unexpected error was returned from Facebook.");
                    }
                    
                    if (!string.IsNullOrWhiteSpace(code))
                    {
                        // Get the user access token
                        string accessToken;
                        if (TryRetrieveFacebookAccessToken(code, returnUrl, result, display, out accessToken))
                        {
                            // Retrieve the user details
                            var fb = new FacebookClient(accessToken);
                            dynamic me = fb.Get("me");

                            // Now we have everything that we need,
                            // work out what we need to do next
                            switch (result)
                            {
                                case "linkaccount":
                                    return ProcessLink(AuthenticationMethod.Facebook, me.id, me.username, returnUrl, display);
                                case "loginorregister":
                                    return ProcessLogin(AuthenticationMethod.Facebook, me.id, me.username, me.name, me.email, accessToken, null, returnUrl, display);
                                case "setup":
                                    return ProcessSetup(AuthenticationMethod.Facebook, me.id, me.username, me.name, me.email, accessToken, null, display);
                                default:
                                    return Error("An unexpected parameter value was found.", "Invalid parameter entered");
                            }
                        }

                        return Error("An unexpected error occurred while retrieving an access token from Facebook.", "Failed to retrieve access token");
                    }

                    return Error("We failed to retrieve all the required information from Facebook.", "Invalid response from Facebook");
                }

                return Error("The anti-forgery token we sent to Facebook didn't match the token we received from Facebook.", "Invalid anti-forgery token");
            }

            // Redirect to Facebook login
            string dialogUrl = BuildFacebookLoginUrl(returnUrl, result, display);
            return new RedirectResult(dialogUrl);
        }

        public ActionResult Login()
        {
            AddHomeBreadcrumb();
            AddBreadcrumb("Login", Url.RouteUrl("AuthenticationSocialLogin"));

            return View();
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private ActionResult ProcessLink(AuthenticationMethod method, string profileId, string profileUsername, string returnUrl, string display)
        {
            var userService = EngineContext.Current.Resolve<IUserService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            if (workContext.CurrentUser == null)
                return Error("An error occurred while adding your additional social media profile, please try again.");

            try
            {
                if (!string.IsNullOrEmpty(profileId) && !string.IsNullOrEmpty(profileUsername))
                {
                    if (userService.UserExists(profileId, method))
                    {
                        // Get the existing user record
                        var user = userService.GetUserByOAuthProfile(profileId, method);
                        bool merge = false;

                        switch (method)
                        {
                            case AuthenticationMethod.Facebook:

                                merge = (user.TwitterProfile == null);

                                break;
                            case AuthenticationMethod.Twitter:

                                merge = (user.FacebookProfile == null);

                                break;
                        }

                        if (merge) // Merge the current user with the additional user
                        {
                            userService.MergeUsers(workContext.CurrentUser, user, method);
                        }
                        else // Cannot merge the accounts
                        {
                            return Error("That " + (method == AuthenticationMethod.Facebook ? "Facebook" : "Twitter") + " account cannot be added to your existing profile because the account is already registered on the site with both a Facebook and Twitter account linked.");
                        }
                    }
                    else
                    {
                        var currentUser = workContext.CurrentUser;

                        // Set the social media profile data
                        if (method == AuthenticationMethod.Facebook)
                        {
                            currentUser.FacebookDisplayName = profileUsername;
                            currentUser.FacebookProfile = profileId;
                        }
                        else
                        {
                            currentUser.TwitterDisplayName = profileUsername;
                            currentUser.TwitterProfile = profileId;
                        }

                        // Update the current user
                        userService.UpdateUser(currentUser);
                    }

                    // Redirect out
                    string redirectUrl = !string.IsNullOrEmpty(returnUrl) ? Server.UrlDecode(returnUrl) : Url.RouteUrl("UserProfile", new { userName = workContext.CurrentUser.UserName });
                    if (display == "popup")
                        return View("redirect", (object)redirectUrl);
                    return Redirect(redirectUrl);
                }

                return Error("We failed to retrieve the required information from " + (method == AuthenticationMethod.Facebook ? "Facebook" : "Twitter") + ", please try again.");
            }
            catch (Exception ex)
            {
                _logService.Error(ex.Message, ex);
                return Error("An error occurred while authenticating with " + (method == AuthenticationMethod.Facebook ? "Facebook" : "Twitter") + ", please try again.");
            }
        }

        private ActionResult ProcessLogin(AuthenticationMethod method, string profileId, string profileUsername, string profileRealName, string profileEmail, string accessToken, string accessSecret, string returnUrl, string display)
        {
            var authService = EngineContext.Current.Resolve<IAuthenticationService>();
            var userService = EngineContext.Current.Resolve<IUserService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            // Sign out the current user
            if (workContext.CurrentUser != null)
                authService.SignOut();

            try
            {
                if (!string.IsNullOrEmpty(profileId) && !string.IsNullOrEmpty(profileUsername))
                {
                    User user;

                    // If the user doesn't exist, create a record
                    if (!userService.UserExists(profileId, method))
                    {
                        string username = profileUsername;
                        int usernameLoopCount = 0;
                        while (userService.UserNameExists(username))
                        {
                            usernameLoopCount++;
                            username = profileUsername + usernameLoopCount;
                        }

                        user = new User
                        {
                            DisplayName = profileRealName,
                            Email = profileEmail,
                            FacebookProfile = (method == AuthenticationMethod.Facebook ? profileId : null),
                            PrimaryAuthMethod = method,
                            TwitterProfile = (method == AuthenticationMethod.Twitter ? profileId : null),
                            UserName = username
                        };

                        // Assign the new user to the member role
                        var memberRole = userService.GetUserRoleBySystemName(SystemUserRoleNames.Members);
                        if (memberRole == null)
                            throw new GatherException("'Members' role could not be loaded");
                        user.UserRoles.Add(memberRole);

                        // Insert the user record
                        userService.InsertUser(user);
                    }

                    // Get the user record
                    user = userService.GetUserByOAuthProfile(profileId, method);
                    user.LastLoginDate = DateTime.Now;

                    // Add related social media profile information
                    switch (method)
                    {
                        case AuthenticationMethod.Facebook:

                            user.FacebookAccessToken = accessToken;
                            user.FacebookDisplayName = profileUsername;

                            break;
                        case AuthenticationMethod.Twitter:

                            user.TwitterAccessSecret = accessSecret;
                            user.TwitterAccessToken = accessToken;
                            user.TwitterDisplayName = profileUsername;

                            break;
                    }

                    // Update the user profile image
                    // Only update if the user's primary method is the method we're using
                    if (user.PrimaryAuthMethod == method)
                    {
                        var profilePicture = GetSocialProfilePicture(method, user.Id, profileUsername, profileId, accessToken);
                        if (!string.IsNullOrEmpty(profilePicture))
                            user.ProfilePicture = profilePicture;
                    }

                    // Update the user
                    userService.UpdateUser(user);

                    // Sign in the user
                    authService.SignIn(user, true);

                    // Redirect the user on
                    string redirectUrl = !string.IsNullOrEmpty(returnUrl) ? Server.UrlDecode(returnUrl) : Url.Action("Index", "Home");
                    if (display == "popup")
                        return View("redirect", (object)redirectUrl);
                    return Redirect(redirectUrl);
                }

                // If we've reached this point, we must be missing data
                return Error("We failed to retrieve the required information from " + (method == AuthenticationMethod.Facebook ? "Facebook" : "Twitter") + ", please try again.");
            }
            catch (Exception ex)
            {
                _logService.Error(ex.Message, ex);
                return Error("An error occurred while authenticating with " + (method == AuthenticationMethod.Facebook ? "Facebook" : "Twitter") + ", please try again.");
            }
        }

        private ActionResult ProcessSetup(AuthenticationMethod method, string profileId, string profileUsername, string profileRealName, string profileEmail, string accessToken, string accessSecret, string display)
        {
            try
            {
                if (!string.IsNullOrEmpty(profileId) && !string.IsNullOrEmpty(profileUsername))
                {
                    var user = new User
                    {
                        DisplayName = profileRealName,
                        Email = profileEmail,
                        FacebookProfile = (method == AuthenticationMethod.Facebook ? profileId : null),
                        PrimaryAuthMethod = method,
                        TwitterProfile = (method == AuthenticationMethod.Twitter ? profileId : null),
                        UserName = profileUsername
                    };

                    // Add related social media profile information
                    switch (method)
                    {
                        case AuthenticationMethod.Facebook:

                            user.FacebookAccessToken = accessToken;
                            user.FacebookDisplayName = profileUsername;

                            break;
                        case AuthenticationMethod.Twitter:

                            user.TwitterAccessSecret = accessSecret;
                            user.TwitterAccessToken = accessToken;
                            user.TwitterDisplayName = profileUsername;

                            break;
                    }

                    // Update the user profile image
                    // Only update if the user's primary method is the method we're using
                    var profilePicture = GetSocialProfilePicture(method, user.Id, profileUsername, profileId, accessToken);
                    if (!string.IsNullOrEmpty(profilePicture))
                        user.ProfilePicture = profilePicture;

                    // Retrieve the existing settings
                    var manager = new DataSettingsManager();
                    var settings = manager.LoadSettings();

                    // Set the user settings
                    settings.SiteOwner = user;

                    // Save the user settings
                    manager.SaveSettings(settings);

                    // Redirect the user on
                    string redirectUrl = Url.Action("stepfour", "install");
                    if (display == "popup")
                        return View("redirect", (object)redirectUrl);
                    return Redirect(redirectUrl);
                }

                // If we've reached this point, we must be missing data
                return Error("We failed to retrieve the required information from " + (method == AuthenticationMethod.Facebook ? "Facebook" : "Twitter") + ", please try again.");
            }
            catch (Exception ex)
            {
                _logService.Error(ex.Message, ex);
                return Error("An error occurred while authenticating with " + (method == AuthenticationMethod.Facebook ? "Facebook" : "Twitter") + ", please try again.");
            }
        }

        public ActionResult Twitter(string display, string result, [Bind(Prefix = "oauth_token")]string oauthToken, [Bind(Prefix = "oauth_verifier")]string oauthVerifier, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(oauthToken) || string.IsNullOrEmpty(oauthVerifier))
                {
                    string callbackUrl = Url.Action("twitter", null, new { returnUrl, result, display }, "http");
                    string token = OAuthUtility.GetRequestToken(_siteSettings.TwitterConsumerKey, _siteSettings.TwitterConsumerSecret, callbackUrl).Token;
                    return new RedirectResult(OAuthUtility.BuildAuthorizationUri(token, true).ToString());
                }

                // Retrieve the user's access token
                var accessToken = OAuthUtility.GetAccessToken(_siteSettings.TwitterConsumerKey, _siteSettings.TwitterConsumerSecret, oauthToken, oauthVerifier);

                // Store the name and user id
                string realName = accessToken.ScreenName;
                string username = accessToken.ScreenName;
                decimal userId = accessToken.UserId;

                // Build the oAuth tokens object
                var tokens = new OAuthTokens
                {
                    AccessToken = accessToken.Token,
                    AccessTokenSecret = accessToken.TokenSecret,
                    ConsumerKey = _siteSettings.TwitterConsumerKey,
                    ConsumerSecret = _siteSettings.TwitterConsumerSecret
                };

                // Try and get the user's real name
                var showUserResponse = TwitterUser.Show(tokens, userId);
                if (showUserResponse.Result == RequestResult.Success)
                    if (!string.IsNullOrEmpty(showUserResponse.ResponseObject.Name))
                        realName = showUserResponse.ResponseObject.Name;

                // Now we have everything that we need,
                // work out what we need to do next
                switch (result)
                {
                    case "linkaccount":
                        return ProcessLink(AuthenticationMethod.Twitter, userId.ToString(), username, returnUrl, display);
                    case "loginorregister":
                        return ProcessLogin(AuthenticationMethod.Twitter, userId.ToString(), username, realName, null, accessToken.Token, accessToken.TokenSecret, returnUrl, display);
                    case "setup":
                        return ProcessSetup(AuthenticationMethod.Twitter, userId.ToString(), username, realName, null, accessToken.Token, accessToken.TokenSecret, display);
                    default:
                        return Error("An unexpected parameter value was found.", "Invalid parameter entered");
                }
            }
            catch (Exception ex)
            {
                _logService.Error(ex.Message, ex);
                return Error("An error occurred while authenticating with Twitter, please try again.", display: display);
            }
        }

        #endregion

        #region Utilities

        private string BuildFacebookLoginUrl(string returnUrl, string result, string display)
        {
            // Generate a state key using the user session ID and the state secret key
            string newState = _webHelper.CalculateMD5Hash(string.Format("{0}-{1}", Session.SessionID, _coreSettings.FacebookStateSecret));

            // Store the users session ID in an encrypted cookie to retrieve when we come back
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, "fbAuth", DateTime.Now, DateTime.Now.AddMinutes(20), false, Session.SessionID, FormsAuthentication.FormsCookiePath);
            string cookie = FormsAuthentication.Encrypt(ticket);
            Response.Cookies.Add(new HttpCookie(FACEBOOK_SESSION_COOKIE, cookie));

            if (display != "page" && display != "popup")
                display = "page";

            string dialogUrl = string.Format("https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&state={2}&scope={3}&display={4}",
                _siteSettings.FacebookAppId,
                Url.Encode(Url.Action("facebook", null, new { returnUrl, result, display }, "http")),
                newState,
                "email,user_photos,publish_stream",
                display);

            return dialogUrl;
        }

        private string BuildFacebookResponseState()
        {
            // Get the forms auth cookie we stored before going to FB
            HttpCookie responseCookie = Request.Cookies.Get(FACEBOOK_SESSION_COOKIE);
            FormsAuthenticationTicket responseTicket = FormsAuthentication.Decrypt(responseCookie.Value);

            // Build the state string we're expecting
            string state = _webHelper.CalculateMD5Hash(string.Format("{0}-{1}", responseTicket.UserData, _coreSettings.FacebookStateSecret));

            // Clear the forms auth cookie
            responseTicket = new FormsAuthenticationTicket(1, "fbAuth", DateTime.Now, DateTime.Now.AddMinutes(-1), false, "", FormsAuthentication.FormsCookiePath);
            string clearCookie = FormsAuthentication.Encrypt(responseTicket);
            Response.Cookies.Add(new HttpCookie(FACEBOOK_SESSION_COOKIE, clearCookie));

            return state;
        }

        private string GetSocialProfilePicture(AuthenticationMethod method, int id, string displayName, string profile, string accessToken)
        {
            try
            {
                string imageUrl = null;

                switch (method)
                {
                    case AuthenticationMethod.Facebook:

                        imageUrl = GetFacebookProfilePictureUrl(accessToken);

                        break;
                    case AuthenticationMethod.Twitter:

                        imageUrl = GetTwitterProfilePictureUrl(displayName);

                        break;
                }

                if (!string.IsNullOrEmpty(imageUrl) && !string.IsNullOrEmpty(profile))
                {
                    string fileExtension = Path.GetExtension(imageUrl);

                    // Default the file extension just in case we didn't get one
                    if (string.IsNullOrEmpty(fileExtension))
                        fileExtension = ".jpg";

                    string fileName = id + "_" + profile + "_large" + fileExtension;

                    var webClient = new WebClient();
                    webClient.DownloadFile(imageUrl, Server.MapPath("~/uploads/profile/") + fileName);

                    return fileName;
                }
            }
            catch (Exception) { }

            return null;
        }

        private string GetFacebookProfilePictureUrl(string accessToken)
        {
            var fb = new FacebookClient(accessToken);
            string pictureUrl = null;

            try
            {
                dynamic albums = fb.Get("me/albums");

                foreach (var album in albums["data"])
                {
                    if (album["type"] == "profile")
                    {
                        dynamic photos = fb.Get(album["id"] + "/photos");
                        pictureUrl = photos["data"][0]["source"];
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return pictureUrl;
        }

        private string GetTwitterProfilePictureUrl(string displayName)
        {
            WebResponse response = null;
            string pictureUrl;

            try
            {
                WebRequest request = WebRequest.Create(string.Format("https://api.twitter.com/1/users/profile_image?screen_name={0}&size=original", displayName));
                response = request.GetResponse();
                pictureUrl = response.ResponseUri.ToString();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (response != null) response.Close();
            }

            return pictureUrl;
        }

        private bool TryRetrieveFacebookAccessToken(string code, string returnUrl, string result, string display, out string accessToken)
        {
            // Default the out parameters
            accessToken = null;

            // Build the Facebook access token URL
            string accessTokenUrl = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}",
                                                  _siteSettings.FacebookAppId,
                                                  Url.Encode(Url.Action("facebook", null, new { returnUrl, result, display }, "http")),
                                                  _siteSettings.FacebookAppSecret,
                                                  code);

            // Create the HTTP request
            var accessTokenRequest = WebRequest.Create(accessTokenUrl);

            try
            {
                // Get the response
                HttpWebResponse accessTokenResponse = (HttpWebResponse)accessTokenRequest.GetResponse();

                // Check we have a 200 response
                if (accessTokenResponse.StatusCode == HttpStatusCode.OK)
                {
                    // Retrieve the access token from the body of the response
                    // Encoding encoding = Encoding.GetEncoding(accessTokenResponse.CharacterSet);
                    using (StreamReader sr = new StreamReader(accessTokenResponse.GetResponseStream()))
                    {
                        accessToken = HttpUtility.ParseQueryString(sr.ReadToEnd()).Get("access_token");
                    }

                    return true;
                }
            }
            catch (Exception) { }

            return false;
        }

        #endregion

    }
}