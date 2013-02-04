using System.Collections.Generic;
using Gather.Web.Models.Api;

namespace Gather.Web.Models.User
{
    public class UserApiModel
    {

        public UserApiModel()
        {
            ApiAuthentication = new ApiAuthenticationModel();
            ShowToken = false;
        }

        public ApiAuthenticationModel ApiAuthentication { get; set; }

        public IList<ApiAuthenticationModel> CurrentApiAuthentication { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }

        public bool ShowToken { get; set; }
    }
}