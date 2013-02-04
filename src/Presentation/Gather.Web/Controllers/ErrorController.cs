using System.Web.Mvc;

namespace Gather.Web.Controllers
{
    public class ErrorController : Controller
    {

        public ActionResult Error403()
        {               
            Response.StatusCode = 403;
            Response.StatusDescription = "Forbidden";
            return View();
        }

        public ActionResult Error404()
        {
            Response.StatusCode = 404;
            Response.StatusDescription = "Not Found";   
            return View();
        }

        public ActionResult Error500()
        {
            Response.StatusCode = 500;
            Response.StatusDescription = "Server Error"; 
            return View();
        }

    }
}