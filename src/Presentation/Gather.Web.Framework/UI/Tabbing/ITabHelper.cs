using System.Collections.Generic;
using System.Web.Mvc;

namespace Gather.Web.Framework.UI.Tabbing
{
    public interface ITabHelper
    {

        string CurrentValue { set; }
        string Param { set; }
        IList<Tab> Tabs { get; set; } 

        void Add(string name, string value);

        string GenerateTab(ref HtmlHelper html, string linkText, string value);

    }
}