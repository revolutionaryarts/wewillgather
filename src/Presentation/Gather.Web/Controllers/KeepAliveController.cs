using System.Web.Mvc;

namespace Gather.Web.Controllers
{
    public class KeepAliveController : Controller
    {

        public ActionResult KeepAlive()
        {
            return Content("The site is running");
        }

    }
}
