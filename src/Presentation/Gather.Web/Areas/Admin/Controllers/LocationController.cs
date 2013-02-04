using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Security;
using Gather.Core.Infrastructure;
using Gather.Core.Seo;
using Gather.Services.Profanities;
using Gather.Services.Security;
using Gather.Web.Areas.Admin.Extensions;
using Gather.Web.Areas.Admin.Models.Common;
using Gather.Web.Areas.Admin.Models.Location;
using Gather.Web.Extensions;
using Gather.Web.Framework.Controllers;
using Gather.Web.Framework.Mvc;
using Gather.Services.Locations;
using Gather.Web.Models.Location;
using Gather.Core.Domain.Locations;
using Gather.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gather.Web.Areas.Admin.Controllers
{
    public class ImportLocation
    {
        public string AdminCode { get; set; }
        public string FeatureCode { get; set; }
        public Location Location { get; set; }
        public int Order { get; set; }
        public string ParentCode { get; set; }
    }

    [AdminAuthorize]
    public class LocationController : ModuleController
    {

        #region Fields

        private readonly CoreSettings _coreSettings;
        private readonly ILocationService _locationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public LocationController() { }

        public LocationController(CoreSettings coreSettings, IPermissionService permissionService, ILocationService locationService, IWorkContext workContext)
        {
            _coreSettings = coreSettings;
            _locationService = locationService;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public ActionResult Import()
        {
            //var importLocations = new List<ImportLocation>();

            //importLocations.Add(new ImportLocation
            //{
            //    AdminCode = "GB",
            //    Location = new Location
            //    {
            //        Latitude = 54.75844m,
            //        Longitude = -2.69531m,
            //        Name = "United Kingdom"
            //    },
            //    Order = 0
            //});

            //string filePath = Server.MapPath("~/App_Data/towns.txt");
            //if (System.IO.File.Exists(filePath))
            //{
            //    string raw = System.IO.File.ReadAllText(filePath);
            //    var rawLocations = raw.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            //    foreach (var location in rawLocations)
            //    {
            //        if (string.IsNullOrEmpty(location))
            //            continue;

            //        var locationParams = location.Split('\t');

            //        if (!locationParams.Any())
            //            continue;

            //        // Exclude anything with a different timezome
            //        if (locationParams[17] != "Europe/London")
            //            continue;

            //        // Only use places and admins
            //        if (locationParams[6] == "A" || locationParams[6] == "P")
            //        {
            //            string adminCode = null;
            //            bool isRegion = false;
            //            int order = 999;
            //            string parentCode = null;

            //            if (locationParams[6] == "A")
            //            {
            //                switch (locationParams[7])
            //                {
            //                    case "ADM1":
            //                        adminCode = locationParams[10];
            //                        parentCode = locationParams[8];
            //                        isRegion = true;
            //                        order = 10;
            //                        break;
            //                    case "ADM1H":
            //                    case "ADM2":
            //                    case "ADM2H":
            //                    case "ADMD":
            //                    case "ADMDH":
            //                        adminCode = !string.IsNullOrEmpty(locationParams[11]) ? locationParams[11] : "00";
            //                        parentCode = locationParams[10];
            //                        order = 20;
            //                        break;
            //                    case "ADM3":
            //                    case "ADM3H":
            //                        adminCode = locationParams[12];
            //                        parentCode = locationParams[11];
            //                        order = 30;
            //                        break;
            //                    case "ADM4":
            //                    case "ADM4H":
            //                        adminCode = locationParams[13];
            //                        parentCode = !string.IsNullOrEmpty(locationParams[12]) ? locationParams[12] : locationParams[11];
            //                        order = 40;
            //                        break;
            //                }
            //            }
            //            else
            //            {
            //                if (!string.IsNullOrEmpty(locationParams[13]))
            //                {
            //                    parentCode = locationParams[13];
            //                    order = 900;
            //                }
            //                else if (!string.IsNullOrEmpty(locationParams[12]))
            //                {
            //                    parentCode = locationParams[12];
            //                    order = 800;
            //                }
            //                else if (!string.IsNullOrEmpty(locationParams[11]))
            //                {
            //                    parentCode = locationParams[11];
            //                    order = 700;
            //                }
            //                else
            //                {
            //                    parentCode = locationParams[10];
            //                    order = 600;
            //                }
            //            }

            //            importLocations.Add(new ImportLocation
            //            {
            //                AdminCode = adminCode,
            //                FeatureCode = locationParams[7],
            //                Location = new Location
            //                {
            //                    IsRegion = isRegion,
            //                    Name = locationParams[1],
            //                    Latitude = decimal.Parse(locationParams[4]),
            //                    Longitude = decimal.Parse(locationParams[5])
            //                },
            //                Order = order,
            //                ParentCode = parentCode
            //            });
            //        }
            //    }
            //}
          
            //importLocations.Add(new ImportLocation
            //{
            //    AdminCode = "EM",
            //    Location = new Location
            //    {
            //        IsRegion = true,
            //        Latitude = 52.9272567392455m,
            //        Longitude = -0.795364659176608m,
            //        Name = "East Midlands"
            //    },
            //    Order = 15,
            //    ParentCode = "ENG"
            //});

            //importLocations.Add(new ImportLocation
            //{
            //    AdminCode = "EE",
            //    Location = new Location
            //    {
            //        IsRegion = true,
            //        Latitude = 52.2448681313924m,
            //        Longitude = 0.548096812180507m,
            //        Name = "East of England"
            //    },
            //    Order = 15,
            //    ParentCode = "ENG"
            //});

            //importLocations.Add(new ImportLocation
            //{
            //    AdminCode = "GL",
            //    Location = new Location
            //    {
            //        IsRegion = true,
            //        Latitude = 51.5005628445244m,
            //        Longitude = -0.109375930039626m,
            //        Name = "London"
            //    },
            //    Order = 15,
            //    ParentCode = "ENG"
            //});

            //importLocations.Add(new ImportLocation
            //{
            //    AdminCode = "NEE",
            //    Location = new Location
            //    {
            //        IsRegion = true,
            //        Latitude = 55.0190863240013m,
            //        Longitude = -1.90405295265715m,
            //        Name = "North East England"
            //    },
            //    Order = 15,
            //    ParentCode = "ENG"
            //});

            //importLocations.Add(new ImportLocation
            //{
            //    AdminCode = "NWE",
            //    Location = new Location
            //    {
            //        IsRegion = true,
            //        Latitude = 54.0505405478878m,
            //        Longitude = -2.72780258268693m,
            //        Name = "North West England"
            //    },
            //    Order = 15,
            //    ParentCode = "ENG"
            //});

            //importLocations.Add(new ImportLocation
            //{
            //    AdminCode = "SEE",
            //    Location = new Location
            //    {
            //        IsRegion = true,
            //        Latitude = 51.2790390252736m,
            //        Longitude = -0.525116000881847m,
            //        Name = "South East England"
            //    },
            //    Order = 15,
            //    ParentCode = "ENG"
            //});

            //importLocations.Add(new ImportLocation
            //{
            //    AdminCode = "SWE",
            //    Location = new Location
            //    {
            //        IsRegion = true,
            //        Latitude = 51.0010615538417m,
            //        Longitude = -3.13081852317805m,
            //        Name = "South West England"
            //    },
            //    Order = 15,
            //    ParentCode = "ENG"
            //});

            //importLocations.Add(new ImportLocation
            //{
            //    AdminCode = "WM",
            //    Location = new Location
            //    {
            //        IsRegion = true,
            //        Latitude = 52.4803840999501m,
            //        Longitude = -2.27087279776346m,
            //        Name = "West Midlands"
            //    },
            //    Order = 15,
            //    ParentCode = "ENG"
            //});

            //importLocations.Add(new ImportLocation
            //{
            //    AdminCode = "YH",
            //    Location = new Location
            //    {
            //        IsRegion = true,
            //        Latitude = 53.9595643889735m,
            //        Longitude = -1.209764814728m,
            //        Name = "Yorkshire and the Humber"
            //    },
            //    Order = 15,
            //    ParentCode = "ENG"
            //});

            //importLocations = importLocations.OrderBy(x => x.Order).ToList();

            //var locations = new List<ImportLocation>();
            //var locationId = 0;

            //foreach (var location in importLocations)
            //{
            //    ImportLocation parent;

            //    if (!string.IsNullOrEmpty(location.ParentCode) && locations.Any(x => x.AdminCode == location.ParentCode))
            //    {
            //        parent = locations.FirstOrDefault(x => x.AdminCode == location.ParentCode);
            //        if (parent != null)
            //            location.Location.ParentLocation = parent.Location;
            //    }

            //    if (location.Order == 20 && location.ParentCode == "ENG")
            //    {
            //        string queryUrl = string.Format("http://nominatim.openstreetmap.org/reverse?lat={0}&lon={1}&format=json&addressdetails=1&zoom=18", location.Location.Latitude.ToString(), location.Location.Longitude.ToString());

            //        try
            //        {
            //            var request = (HttpWebRequest)WebRequest.Create(queryUrl);
            //            var response = (HttpWebResponse)request.GetResponse();
            //            var stream = response.GetResponseStream();
            //            if (stream != null)
            //            {
            //                using (var streamReader = new StreamReader(stream))
            //                {
            //                    var json = streamReader.ReadToEnd();
            //                    var parsed = JObject.Parse(json);
            //                    if (parsed != null)
            //                    {
            //                        string region = parsed["address"]["state_district"].ToString();
            //                        if (region == "London")
            //                            region = "Greater London";

            //                        parent = locations.FirstOrDefault(x => x.Location.Name.ToLower() == region.ToLower() && x.Order == 15);
            //                        if (parent != null)
            //                            location.Location.ParentLocation = parent.Location;
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception) { }
            //    }

            //    // Manual overrides
            //    switch (location.Location.Name)
            //    {
            //        case "Bath and North East Somerset":
            //        case "Borough of Bournemouth":
            //        case "Borough of Poole":
            //        case "Dorset":
            //        case "Isles of Scilly":
            //        case "South Gloucestershire":
            //        case "Wiltshire":
            //            parent = importLocations.FirstOrDefault(x => x.Location.Name == "South West England" && x.Order == 15);
            //            if (parent != null)
            //                location.Location.ParentLocation = parent.Location;
            //            break;
            //        case "Shaw":
            //            if (location.Location.ParentLocation != null && location.Location.ParentLocation.Id == 5)
            //                continue;
            //            break;
            //        case "Shipley":
            //            if (location.Location.ParentLocation != null && location.Location.ParentLocation.Id == 5)
            //            {
            //                parent = importLocations.FirstOrDefault(x => x.Location.Name == "Yorkshire and the Humber" && x.Order == 15);
            //                if (parent != null)
            //                    location.Location.ParentLocation = parent.Location;
            //            }
            //            break;
            //        case "Purston Jaglin":
            //        case "Thamesdown":
            //            continue;
            //    }

            //    if(location.Location.ParentLocation == null || !locations.Any(x => x.Location.Name == location.Location.Name && x.Location.ParentLocation.Id == location.Location.ParentLocation.Id))
            //    {
            //        locationId++;
            //        location.Location.Id = locationId;

            //        if (location.Order == 0 || location.Location.ParentLocation != null)
            //            locations.Add(location);
            //    }
            //}

            //var locationSet = new DataSet("Locations");

            //var locationTable = new DataTable("Location");
            //locationTable.Columns.Add("Id", typeof(int));
            //locationTable.Columns.Add("Name", typeof(string));
            //locationTable.Columns.Add("ParentLocation_Id", typeof(string));
            //locationTable.Columns.Add("SeoName", typeof(string));
            //locationTable.Columns.Add("IsRegion", typeof(bool));
            //locationTable.Columns.Add("Latitude", typeof(decimal));
            //locationTable.Columns.Add("Longitude", typeof(decimal));

            //locationSet.Tables.Add(locationTable);

            //foreach (var location in locations)
            //{
            //    var row = locationSet.Tables[0].NewRow();
            //    row["Id"] = location.Location.Id;
            //    row["Name"] = location.Location.Name;
            //    row["ParentLocation_Id"] = (location.Location.ParentLocation == null ? "" : location.Location.ParentLocation.Id.ToString());
            //    row["SeoName"] = location.Location.GetSeoName();
            //    row["IsRegion"] = location.Location.IsRegion;
            //    row["Latitude"] = location.Location.Latitude;
            //    row["Longitude"] = location.Location.Longitude;
            //    locationSet.Tables[0].Rows.Add(row);
            //}

            //var sbXML = new StringBuilder();
            //sbXML.Append(@"<?xml version=""1.0"" encoding=""windows-1252"" ?>");
            //sbXML.Append(Environment.NewLine);

            //var sw = new StringWriter(sbXML);
            //locationSet.WriteXml(sw, XmlWriteMode.WriteSchema);

            //var file = new StreamWriter(Server.MapPath(@"~/App_Data/locations.txt"));
            //file.WriteLine(sbXML.ToString());
            //file.Close();

            return View();
        }

        public ActionResult List(int id = 0)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageLocations))
                return AccessDeniedView();

            var locations = _locationService.GetAllLocations(Page, _coreSettings.AdminGridPageSize, id, null, Search);

            var model = new LocationListModel
            {
                PageIndex = Page,
                PageSize = _coreSettings.AdminGridPageSize,
                TotalCount = locations.TotalCount,
                TotalPages = locations.TotalPages,
                Search = Search,
                Locations = locations.Select(PrepareListLocationModel).ToList()
            };

            if(string.IsNullOrEmpty(Search))
                PrepareBreadcrumbs(model.Locations[0].ParentLocation);
            else
                PrepareBreadcrumbs(isSearch: true);

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageLocations))
                return AccessDeniedView();

            var location = _locationService.GetLocationById(id);
            if (location == null)
                return RedirectToAction("list", new { page = Page, search = Search });

            try
            {
                _locationService.DeleteLocation(location);
                SuccessNotification(location.Name + " has been successfully deleted.");
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred deleting " + location.Name + ", please try again");
            }

            return RedirectToAction("list", new { id = location.ParentLocation.Id, page = Page, search = Search });
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageLocations))
                return AccessDeniedView();

            // get the category
            var location = _locationService.GetLocationById(id);

            // check we have a category and they are not deleted
            if (location == null || location.Deleted)
                return RedirectToAction("list");

            if (!location.Active)
                WarningNotification("This location is currently hidden. To show it, use the 'show' button on the right.");

            var model = location.ToModel();

            PrepareBreadcrumbs(model.ParentLocation);
            AddBreadcrumb("Edit Location", null);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Edit(LocationModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageLocations))
                return AccessDeniedView();

            // get the location
            var location = _locationService.GetLocationById(model.Id);

            // check we have a location and they are not deleted
            if (location == null || location.Deleted)
                return RedirectToAction("list");
            location.LastModifiedBy = _workContext.CurrentUser.Id;

            if (ModelState.IsValid)
            {
                try
                {
                    location.HashTag = model.HashTag;
                    _locationService.UpdateLocation(location);

                    SuccessNotification("The location details have been updated successfully.");
                    return RedirectToAction("Edit", location.Id);
                }
                catch (Exception)
                {
                    ErrorNotification("An error occurred saving the user details, please try again.");
                }
            }

            PrepareBreadcrumbs(model.ParentLocation);
            AddBreadcrumb("Edit Location", null);

            return View(model);
        }

        private void PrepareBreadcrumbs(LocationModel parentLocation = null, bool isSearch = false)
        {
            AddBreadcrumb("Locations", "list", "location", new { area = "admin" });

            if (isSearch)
            {
                AddBreadcrumb("Search Results", null);
            }
            else
            {
                var currentLocation = parentLocation;
                while (currentLocation != null)
                {
                    AddBreadcrumb(currentLocation.Name, Url.Action("list", new {area = "admin", id = currentLocation.Id}), 1);
                    currentLocation = currentLocation.ParentLocation;
                }
            }
        }

        #endregion

        #region Prepare Models

        [NonAction]
        private LocationModel PrepareListLocationModel(Location location)
        {
            var model = location.ToModel();

            model.Actions.Add(new ModelActionLink
            {
                Alt = "Edit",
                Icon = Url.Content("~/Areas/Admin/Content/images/icon-edit.png"),
                Target = Url.Action("edit", new { id = location.Id })
            });

            if (location.ChildLocations.Count > 0)
            {
                model.Actions.Add(new ModelActionLink
                {
                    Alt = "Delete",
                    Icon = Url.Content("~/Areas/Admin/Content/images/sitemap-application-blue.png"),
                    Target = Url.Action("list", new { id = location.Id })
                });
            }

            return model;
        }

        #endregion

        #region Navigation

        public override IList<NavigationSectionModel> RegisterNavigation()
        {
            var sections = new List<NavigationSectionModel>();

            HttpContextWrapper httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, new RouteData()));

            var section = new NavigationSectionModel
            {
                Icon = urlHelper.Content("~/Areas/Admin/Content/images/menu-locations.png"),
                Name = "location",
                Position = 3,
                RequiredPermissions = new List<PermissionRecord>
                {
                    PermissionProvider.ManageLocations  
                },
                Target = urlHelper.Action("list", "location", new { area = "admin" }),
                Title = "Locations"
            };

            section.AddChildLink("Manage Locations", urlHelper.Action("list", "location", new { area = "admin" }));
            sections.Add(section);

            return sections;
        }

        #endregion

    }
}