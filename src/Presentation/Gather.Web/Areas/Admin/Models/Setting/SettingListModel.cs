using System.Collections.Generic;
using Gather.Web.Framework.Mvc;
using Gather.Web.Models.Setting;

namespace Gather.Web.Areas.Admin.Models.Setting
{
    public class SettingListModel : BaseAdminListModel
    {
        public IList<SettingModel> Settings { get; set; }
    }
}