using System.Collections.Generic;

namespace Gather.Web.Framework.UI.Breadcrumbs
{
    public class BreadcrumbHelper : IBreadcrumbHelper
    {

        #region Properties

        public IList<Breadcrumb> Breadcrumbs { get; set; }

        #endregion

        #region Constructors

        public BreadcrumbHelper()
        {
            Breadcrumbs = new List<Breadcrumb>();
        }

        #endregion

        #region Methods

        public void Add(string title, string target, int? index = null)
        {
            if(index == null)
                Breadcrumbs.Add(new Breadcrumb(title, target));
            else
                Breadcrumbs.Insert((int)index, new Breadcrumb(title, target));
        }

        #endregion

    }
}