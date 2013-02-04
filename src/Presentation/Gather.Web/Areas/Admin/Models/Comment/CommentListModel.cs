using System.Collections.Generic;
using Gather.Web.Framework.Mvc;
using Gather.Web.Models.Comment;

namespace Gather.Web.Areas.Admin.Models.Comment
{
    public class CommentListModel : BaseAdminListModel
    {
        public IList<CommentModel> Comments { get; set; } 
    }
}