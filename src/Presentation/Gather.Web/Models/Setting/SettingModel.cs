using Gather.Web.Areas.Admin.Models;

namespace Gather.Web.Models.Setting
{
    public class SettingModel : BaseModel
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}