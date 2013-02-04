namespace Gather.Web.Framework.UI.Breadcrumbs
{
    public class Breadcrumb
    {
        public Breadcrumb(string title, string target)
        {
            Title = title;
            Target = target;
        }

        public string Target { get; set; }
        public string Title { get; set; }
    }
}