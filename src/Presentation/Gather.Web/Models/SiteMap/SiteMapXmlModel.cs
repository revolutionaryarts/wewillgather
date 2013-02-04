using System.Collections.Generic;
using Gather.Web.Areas.Admin.Models;
using Gather.Web.Models.Location;
using Gather.Web.Models.Page;
using Gather.Web.Models.SuccessStory;
using Gather.Web.Models.Project;

namespace Gather.Web.Models.SiteMap
{
    public class SiteMapXmlModel : BaseModel
    {
        public SiteMapXmlModel()
        {
            Nodes = new List<XmlNode>();
        }

        public IList<XmlNode> Nodes { get; set; }
        public IList<SuccessStoryModel> SuccessStories { get; set; }
        public IList<PageModel> Pages { get; set; }
        public IList<LocationModel> Locations { get; set; }
        public IList<BaseProjectModel> Projects { get; set; }
    }

    public class XmlNode
    {
        public XmlNode(string url, decimal priority)
        {
            Url = url;
            Priority = priority;
        }

        public decimal Priority { get; set; }

        public string Url { get; set; }
    }

}