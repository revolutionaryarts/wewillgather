using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gather.Core;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Messages;
using Gather.Services.MessageQueues;
using Gather.Services.Users;
using Gather.Web.Extensions;
using Gather.Web.Models.Contact;
using Gather.Web.Models.User;

namespace Gather.Web.Controllers
{
    public class ContactController : BaseController
    {

        #region Fields

        private readonly IMessageQueueService _messageQueueService;
        private readonly SiteSettings _siteSettings;
        private readonly IUserService _userService;

        #endregion

        #region Constructor

        public ContactController(IMessageQueueService messageQueueService, SiteSettings siteSettings, IUserService userService)
        {
            _messageQueueService = messageQueueService;
            _siteSettings = siteSettings;
            _userService = userService;
        }

        #endregion

        #region Methods

        public ActionResult Contact()
        {
            PrepareBreadcrumbs();

            var model = new ContactModel
            {
                Admins = _userService.GetAllUsersForContactUs().Select(x => x.ToModel()).ToList(),
                DisplayError = false
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Contact(ContactModel model)
        {
            PrepareBreadcrumbs();

            model.DisplayError = !ModelState.IsValid;
            if (ModelState.IsValid)
            {
                var message = new MessageQueue {
                    Priority = 1,
                    User = _userService.GetSiteOwner(),
                    Subject = _siteSettings.TwitterHashTag + " - Contact Message",
                    Body = RenderRazorViewToString("~/Views/Templates/Contact.cshtml", model)
                };

                _messageQueueService.InsertMessageQueue(message);
                return RedirectToRoute("ContactThanks");
            }

            model.Admins = _userService.GetAllUsersForContactUs().Select(x => x.ToModel()).ToList();
            return View(model);
        }

        public ActionResult ContactThanks()
        {
            return View();
        }

        public ActionResult UserContact(UserModel user)
        {
            var model = new Dictionary<string, string>
            {
                { "Profile", Url.RouteUrl("UserProfile", new {username = user.UserName}) }
            };

            if (user.FacebookProfile != null)
                model.Add("Facebook", string.Format("http://www.facebook.com/profile.php?id={0}", user.FacebookProfile));

            if (user.TwitterProfile != null)
                model.Add("Twitter", string.Format("http://twitter.com/account/redirect_by_id?id={0}", user.TwitterProfile));

            if (!string.IsNullOrEmpty(user.Email) && user.EmailDisclosureLevel == DisclosureLevel.Public)
                model.Add("Email", string.Format("mailto:{0}", user.Email.EncodeEmail()));

            if (!string.IsNullOrEmpty(user.Telephone) && user.TelephoneDisclosureLevel == DisclosureLevel.Public)
                model.Add("Call", string.Format("tel:{0}", user.Telephone));

            if (!string.IsNullOrEmpty(user.Website) && user.WebsiteDisclosureLevel == DisclosureLevel.Public)
                model.Add("Website", user.Website);

            return PartialView("_UserContact", model);
        }

        private void PrepareBreadcrumbs()
        {
            AddHomeBreadcrumb();
            AddBreadcrumb("Contact Us", Url.RouteUrl("Contact"));
        }

        #endregion

    }
}