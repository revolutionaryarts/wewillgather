using System.Collections.Generic;
using System.Reflection;

namespace Gather.Core.Infrastructure
{
    public class WebAppTypeFinder : AppDomainTypeFinder
    {

        private bool _ensureBinFolderAssembliesLoaded = true;
        private bool _binFolderAssembliesLoaded;

        private readonly IWebHelper _webHelper;

        public WebAppTypeFinder(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }

        #region Properties

        /// <summary>
        /// Gets or sets wether assemblies in the bin folder of the web application should be specificly checked for beeing loaded on application load. This is need in situations where plugins need to be loaded in the AppDomain after the application been reloaded.
        /// </summary>
        public bool EnsureBinFolderAssembliesLoaded
        {
            get { return _ensureBinFolderAssembliesLoaded; }
            set { _ensureBinFolderAssembliesLoaded = value; }
        }

        #endregion

        #region Methods

        public override IList<Assembly> GetAssemblies()
        {
            if (EnsureBinFolderAssembliesLoaded && !_binFolderAssembliesLoaded)
            {
                _binFolderAssembliesLoaded = true;
                LoadMatchingAssemblies(_webHelper.MapPath("~/bin"));
            }

            return base.GetAssemblies();
        }

        #endregion

    }
}