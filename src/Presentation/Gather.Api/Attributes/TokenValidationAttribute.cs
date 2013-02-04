using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Gather.Api.Attributes
{    
    public class TokenValidationAttribute : ActionFilterAttribute
    {        
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                string token = actionContext.Request.Headers.GetValues("Authorization").First();
            }
            catch (Exception)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("[{\"Error\":[\"Missing Access-Token\"]}]")
                };
                return;
            }

            try
            {
                base.OnActionExecuting(actionContext);
            }
            catch (Exception)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {                    
                    Content = new StringContent("[{\"Error\":[\"Unauthorised User\"]}]")
                };
            }
        }
    }
}