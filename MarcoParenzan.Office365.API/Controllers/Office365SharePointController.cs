using MarcoParenzan.Office365.API.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MarcoParenzan.Office365.API.Controllers
{
    public class Office365SharePointController : Controller
    {
        /// <summary>
        /// Displays upcoming calendar events, from the user's default calendar.
        /// Minimal permission required: permission to read users' calendars.
        /// </summary>
        public async Task<ActionResult> Files()
        {
            // Obtain information for communicating with the service:
            Office365ServiceInfo serviceInfo = await Office365ServiceInfo.GetSharePointOneDriveServiceInfoAsync();
            if (!serviceInfo.HasValidAccessToken)
            {
                return Redirect(serviceInfo.GetAuthorizationUrl(Request.Url));
            }

            // Create a URL for retrieving the data:
            string[] queryParameters =
            { 
                "$select=Name"
            };
            string requestUrl = String.Format(CultureInfo.InvariantCulture,
                "{0}/Files?{1}",
                serviceInfo.ApiEndpoint,
                String.Join("&", queryParameters));

            // Prepare the HTTP request:
            using (HttpClient client = new HttpClient())
            {
                Func<HttpRequestMessage> requestCreator = () =>
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                    request.Headers.Add("Accept", "application/json");
                    return request;
                };

                // Send the request using a helper method, which will add an authorization header to the request,
                // and automatically retry with a new token if the existing one has expired.
                using (HttpResponseMessage response = await Office365Controller.SendRequestAsync(
                    serviceInfo, client, requestCreator))
                {
                    // Read the response and deserialize the data:
                    string responseString = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        return Office365Controller.ShowErrorMessage(serviceInfo, responseString);
                    }
                    var files = JObject.Parse(responseString)["value"].ToObject<SharePointFile[]>();

                    return Json(files, JsonRequestBehavior.AllowGet);
                }
            }

        // TODO: For your users' security, be sure to include Office365CommonController.ClearSession()
        //       as part of your application's signout routine.
        }
    }
}
