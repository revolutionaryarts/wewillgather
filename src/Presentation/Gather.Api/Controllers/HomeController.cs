using System.Web.Mvc;

namespace Gather.Api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectPermanent("http://www.wewillgather.co.uk/developers");
        }
    }
}