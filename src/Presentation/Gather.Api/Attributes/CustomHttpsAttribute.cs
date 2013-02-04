using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Gather.Api.Attributes
{
    public class CustomHttpsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //if (!String.Equals(actionContext.Request.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            //{
            //    actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            //    {
            //        Content = new StringContent("[{\"Error\":[\"HTTPS Required\"]}]")
            //    };
            //    return;
            //}
        }
    }
}