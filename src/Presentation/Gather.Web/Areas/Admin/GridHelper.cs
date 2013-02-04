using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Gather.Core.Domain.Users;
using Gather.Web.Areas.Admin.Models;
using Gather.Web.Models.User;

namespace Gather.Web.Areas.Admin
{
    public static class GridHelper
    {

        public static IHtmlString ActionColumn<T>(T model) where T : BaseModel
        {
            var sb = new StringBuilder();

            foreach (var action in model.Actions)
            {
                TagBuilder link = new TagBuilder("a");
                link.Attributes.Add("href", action.Target);

                foreach (var attribute in action.AdditionalAttributes)
                    link.Attributes.Add(attribute.Key, attribute.Value);

                foreach (var className in action.Classes)
                    link.AddCssClass(className);

                link.AddCssClass("action-link");

                TagBuilder image = new TagBuilder("image");
                image.Attributes.Add("alt", action.Alt);
                image.Attributes.Add("src", action.Icon);

                link.InnerHtml = image.ToString();

                sb.Append(link.ToString());
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static IHtmlString LinkColumn(string title, string target)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(target))
                return null;

            var link = "<a href=" + target + ">" + title + "</a>";

            return MvcHtmlString.Create(link);
        }

        public static IHtmlString UserNameColumn<T>(T model) where T : UserModel
        {
            var sb = new StringBuilder(model.UserName);

            if(model.UserRoles.Any(x => x.SystemName == SystemUserRoleNames.SiteOwner))
            {
                TagBuilder crown = new TagBuilder("img");
                crown.Attributes.Add("src", "/areas/admin/content/images/crown.png");
                crown.Attributes.Add("alt", "Site Owner");
                crown.Attributes.Add("class", "site-owner-crown");

                sb.Append(crown.ToString());
            }

            return MvcHtmlString.Create(sb.ToString());
        }

    }
}