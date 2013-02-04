using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using AttributeRouting.Web.Http;
using Gather.Api.Extensions;
using Gather.Api.Models;
using Gather.Core;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Projects;
using Gather.Services.ApiAuthentications;
using Gather.Services.Locations;
using Gather.Services.Projects;

namespace Gather.Api.Controllers
{
    public class ProjectController : ApiController
    {

        #region Variables

        private readonly IProjectService _projectService;
        private readonly ILocationService _locationService;
        private readonly IApiAuthenticationService _apiAuthenticationService;

        #endregion

        #region Constructors

        public ProjectController(IProjectService projectService, ILocationService locationService, IApiAuthenticationService apiAuthenticationService)
        {
            _projectService = projectService;
            _locationService = locationService;
            _apiAuthenticationService = apiAuthenticationService;
        }

        #endregion

        #region Methods

        [GET(@"projects/")]
        public IEnumerable<ProjectModel> GetProjects(int pageIndex = 1, int pageSize = 10, string search = null, decimal latitude = 0, decimal longitude = 0, int? radius = 5, string locationName = null)
        {
            var tokenState = _apiAuthenticationService.ValidateToken(Request.Headers.GetValues("Authorization").First());

            if (pageSize < -1 || pageSize == 0)
            {
                pageSize = -1;
            }

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            Match match = Regex.Match(radius.ToString(), @"(5|10|15|50|0)$", RegexOptions.IgnoreCase);
	        if (!match.Success)
	        {
	            radius = 5;
	        }            

            switch (tokenState)
            {
                case ApiAuthenticationType.Valid:

                    if (locationName != null)
                    {
                        var location = _locationService.FindLocationByName(HttpUtility.UrlDecode(locationName), true);
                        if (location == null)
                        {
                            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                            {
                                Content = new StringContent("[{\"Error\":[\"We couldn't find a location for: " + locationName + "\"]}]")
                            });
                        }

                        var projects = _projectService.GetAllProjectsByLocationPaged(location, pageIndex, pageSize).Select(x => x.ToModel()).ToList();
                        if (!projects.Any())
                        {
                            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NoContent));
                        }

                        return projects;
                    }
                    
                    if (latitude != 0 && longitude != 0)
                    {
                        ProjectSearchRadius searchRadius;
                        if (!radius.HasValue)
                            searchRadius = ProjectSearchRadius.AnyDistance;
                        else
                            searchRadius = (ProjectSearchRadius)Enum.Parse(typeof(ProjectSearchRadius), radius.Value.ToString());

                        var projects = _projectService.GetAllProjectsByCoordsPaged(latitude, longitude, pageIndex, pageSize, null, searchRadius).Select(x => x.ToModel()).ToList();
                        if (!projects.Any())
                        {
                            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NoContent));
                        }

                        return projects;
                    }
                    else
                    {
                        var projects = _projectService.GetAllProjects(pageIndex, pageSize, (int)ProjectStatus.Open, HttpUtility.UrlDecode(search)).Select(x => x.ToModel()).ToList();

                        if (!projects.Any())
                        {
                            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NoContent));
                        }

                        return projects;
                    }

                default:

                    var errorMessage = new HttpResponseMessage(HttpStatusCode.Forbidden) { Content = new StringContent("[{\"Error\":[\"" + tokenState.GetDescription() + "\"]}]") };
                    throw new HttpResponseException(errorMessage);

            }
        }

        #endregion

    }
}