using System.IO;
using System.Net;
using System.Web.Mvc;
using Gather.Services.Security;

namespace Gather.Web.Controllers
{
    public class ApiController : Controller
    {
        private readonly IEncryptionService _encryptionService;

        public ApiController(IEncryptionService encryptionService)
        {            
            _encryptionService = encryptionService;
        }

        public ActionResult Index()
        {

            string accessToken = "iajTWqTGt5e4/ZB+lSfvoviJPPE/iiuVd94qlnoafSQBojYJ5EsZgPbMoyG9W+AjZL8KX5mE8eOdQslE4GQ3zqds+Wfu1wk3";

            // Create a request for the URL. 
            HttpWebRequest request = (HttpWebRequest)GetRequest("GET", "http://api.wewillgather.freshegg.lom/projects/", accessToken);

            try
            {
                HttpWebResponse response;
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Get the response.
                        using (var dataStream = response.GetResponseStream())
                        {
                            // Open the stream using a StreamReader for easy access.
                            if (dataStream != null)
                            {
                                using (var reader = new StreamReader(dataStream))
                                {
                                    // Read the content.
                                    string responseFromServer = reader.ReadToEnd();
                                    // Display the content.

                                    Response.Write(responseFromServer);
                                }                         
                            }
                        }
                        // Clean up the streams and the response.                        
                        response.Close();
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    // can use ex.Response.Status, .StatusDescription
                    if (ex.Response.ContentLength != 0)
                    {
                        using (var stream = ex.Response.GetResponseStream())
                        {
                            if (stream != null)
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    Response.Write(reader.ReadToEnd());
                                }
                            }
                        }
                    }
                    ex.Response.Close();
                }   
            }

            return View();
        }

        static WebRequest GetRequest(string method, string endPoint, string accessToken)
        {
            var request = WebRequest.Create(endPoint);
            request.Method = method;            

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            request.Headers.Add("Authorization", accessToken);

            return request;
        }

    }
}
