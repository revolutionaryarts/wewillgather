using Gather.Web.Areas.Admin.Models;
using System;

namespace Gather.Web.Models.Tweet
{
    public class TweetModel : BaseModel
    {
        public DateTime CreatedDate { get; set; }

        public string CreatedDateString { get; set; }

        public string DateDifference { get; set; }
    
        public string Text { get; set; }

        public string TwitterProfile { get; set; }

        public string TwitterId { get; set; }

        public string TwitterName { get; set; }

        public string UserName { get; set; }
    }
}