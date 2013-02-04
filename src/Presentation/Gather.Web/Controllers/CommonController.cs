using System.Collections.Generic;
using System.Web.Mvc;
using Gather.Core;
using Gather.Core.Configuration;
using Gather.Core.Domain.Common;
using Gather.Services.Geolocation;
using Gather.Services.Security;
using Gather.Web.Extensions;
using Gather.Web.Framework.UI;
using Gather.Web.Models.Common;
using Gather.Web.Models.Contact;

namespace Gather.Web.Controllers
{
    public class CommonController : Controller
    {

        #region Fields

        private readonly IGeolocationService _geolocationService;
        private readonly IPermissionService _permissionService;
        private readonly SiteSettings _siteSettings;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public CommonController(IGeolocationService geolocationservice, IPermissionService permissionService, SiteSettings siteSettings, IWorkContext workContext)
        {
            _geolocationService = geolocationservice;
            _permissionService = permissionService;
            _siteSettings = siteSettings;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public ActionResult Breadcrumbs()
        {
            var controller = ControllerContext.ParentActionViewContext.RouteData.Values["controller"].ToString().ToLower();
            var action = ControllerContext.ParentActionViewContext.RouteData.Values["action"].ToString().ToLower();

            var isHome = (controller == "home" && action == "index");
            return PartialView("_Breadcrumbs", isHome);
        }

        public ActionResult FacebookWidget()
        {
            string profileId = _siteSettings.FacebookWidgetProfileId;
            return PartialView("_FacebookWidget", profileId);
        }

        public ActionResult FlickrWidget()
        {
            var model = new FlickrWidgetModel
            {
                UserId = _siteSettings.FlickrWidgetId,
                UserUrl = _siteSettings.FlickrWidgetUrl
            };

            return PartialView("_FlickrWidget", model);
        }

        public ActionResult GoogleAnalyticsTracking()
        {
            var model = new GoogleAnalyticsTrackingModel
            {
                EnableGoogleAnalyticsTracking = _siteSettings.GoogleAnalyticsEnabled == "true",
                GoogleAnalyticsTrackingCode = _siteSettings.GoogleAnalyticsUACode
            };

            return PartialView("_GoogleAnalyticsTracking", model);
        }

        public ActionResult LatLngLookup(string search)
        {
            if (!string.IsNullOrEmpty(search) && search.ToLower() != "location or postcode")
            {
                decimal latitude;
                decimal longitude;

                _geolocationService.GetLatLng(search, out latitude, out longitude);

                List<decimal> coords = null;
                
                if(latitude != 999 && longitude != 999)
                    coords = new List<decimal> { latitude, longitude };

                return Json(coords, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult LocationLookup(decimal latitude, decimal longitude)
        {
            var location = _geolocationService.GetLocationFromLatLng(latitude, longitude);
            return Json(location != null ? location.Name : "Unknown", JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoginStatus()
        {
            var model = new LoginStatusModel
            {
                CurrentUser = _workContext.CurrentUser.ToModel(),
                IsAdmin = _permissionService.Authorize(PermissionProvider.AccessAdminArea)
            };

            return PartialView("_LoginStatus", model);
        }

        public ActionResult LoggedInState()
        {
            var model = new LoggedInStateModel
            {
                CurrentUser = _workContext.CurrentUser.ToModel(),
                IsAdmin = _permissionService.Authorize(PermissionProvider.AccessAdminArea)
            };

            return PartialView("_LoggedInState", model);
        }

        public ActionResult Notifications()
        {
            var model = new NotificationModel();

            if (TempData[string.Format("gather.notifications.{0}", NotifyType.Error)] != null)
                model.ErrorMessages.AddRange(TempData[string.Format("gather.notifications.{0}", NotifyType.Error)] as IList<string>);

            if (ViewData[string.Format("gather.notifications.{0}", NotifyType.Error)] != null)
                model.ErrorMessages.AddRange(ViewData[string.Format("gather.notifications.{0}", NotifyType.Error)] as IList<string>);

            if (TempData[string.Format("gather.notifications.{0}", NotifyType.Success)] != null)
                model.SuccessMessages.AddRange(TempData[string.Format("gather.notifications.{0}", NotifyType.Success)] as IList<string>);

            if (ViewData[string.Format("gather.notifications.{0}", NotifyType.Success)] != null)
                model.SuccessMessages.AddRange(ViewData[string.Format("gather.notifications.{0}", NotifyType.Success)] as IList<string>);

            if (TempData[string.Format("gather.notifications.{0}", NotifyType.Warning)] != null)
                model.WarningMessages.AddRange(TempData[string.Format("gather.notifications.{0}", NotifyType.Warning)] as IList<string>);

            if (ViewData[string.Format("gather.notifications.{0}", NotifyType.Warning)] != null)
                model.WarningMessages.AddRange(ViewData[string.Format("gather.notifications.{0}", NotifyType.Warning)] as IList<string>);

            return PartialView("_Notifications", model);
        }

        #endregion

    }
}