using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gather.Web.Models.Common
{
    public class NotificationModel
    {
        public NotificationModel()
        {
            ErrorMessages = new List<string>();
            SuccessMessages = new List<string>();
            WarningMessages = new List<string>();
        }

        public List<string> ErrorMessages { get; set; }

        public List<string> SuccessMessages { get; set; }

        public List<string> WarningMessages { get; set; }
    }
}