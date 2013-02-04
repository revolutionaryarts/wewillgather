using System.Collections.Generic;
using Gather.Web.Framework.Mvc;

namespace Gather.Web.Areas.Admin.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
            Actions = new List<ModelActionLink>();
        }

        public int Id { get; set; }
        public IList<ModelActionLink> Actions { get; set; }
    }
}