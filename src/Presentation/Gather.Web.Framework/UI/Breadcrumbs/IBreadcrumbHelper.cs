using System.Collections.Generic;

namespace Gather.Web.Framework.UI.Breadcrumbs
{
    public interface IBreadcrumbHelper
    {

        #region Properties

        IList<Breadcrumb> Breadcrumbs { get; set; }

        #endregion

        #region Methods

        void Add(string title, string target, int? index = null);

        #endregion

    }
}