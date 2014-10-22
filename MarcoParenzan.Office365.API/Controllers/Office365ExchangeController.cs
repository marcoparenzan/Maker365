using MarcoParenzan.Office365.API.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MarcoParenzan.Office365.API.Controllers
{
    public class Office365ExchangeController : Controller
    {
        /// <summary>
        /// Displays upcoming calendar events, from the user's default calendar.
        /// Minimal permission required: permission to read users' calendars.
        /// </summary>
        public async Task<ActionResult> Calendar()
        {
            // Obtain information for communicating with the service:
            Office365ServiceInfo serviceInfo = Office365ServiceInfo.GetExchangeServiceInfo();
            if (!serviceInfo.HasValidAccessToken)
            {
                return Redirect(serviceInfo.GetAuthorizationUrl(Request.Url));
            }

            // Create a URL for retrieving the data:
            string[] queryParameters = 
            {
                String.Format(CultureInfo.InvariantCulture, "$filter=End ge {0}Z", DateTime.UtcNow.ToString("s")),
                "$top=10",
                "$select=Subject,Start,End"
            };
            string requestUrl = String.Format(CultureInfo.InvariantCulture,
                "{0}/Me/Calendar/Events?{1}",
                serviceInfo.ApiEndpoint,
                String.Join("&", queryParameters));

            // Prepare the HTTP request:
            using (HttpClient client = new HttpClient())
            {
                Func<HttpRequestMessage> requestCreator = () =>
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                    //request.Headers.Add("Accept", "application/json;odata=minimalmetadata");
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
                    var events = JObject.Parse(responseString)["value"].ToObject<CalendarEvent[]>();
                    events = events.OrderBy(e => e.Start).ToArray();

                    return Json(events, JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// Displays recent messages in the user's inbox.
        /// Minimal permission required: permission to read users' mail.
        /// </summary>
        public async Task<ActionResult> Mail()
        {
            // Obtain information for communicating with the service:
            Office365ServiceInfo serviceInfo = Office365ServiceInfo.GetExchangeServiceInfo();
            if (!serviceInfo.HasValidAccessToken)
            {
                return Redirect(serviceInfo.GetAuthorizationUrl(Request.Url));
            }

            // Create a URL for retrieving the data:
            string[] queryParameters = 
            {
                "$orderby=DateTimeSent desc",
                "$top=20",
                "$select=Subject,DateTimeReceived,From"
            };
            string requestUrl = String.Format(CultureInfo.InvariantCulture,
                "{0}/Me/Folders/Inbox/Messages?{1}",
                serviceInfo.ApiEndpoint,
                String.Join("&", queryParameters));

            // Prepare the HTTP request:
            using (HttpClient client = new HttpClient())
            {
                Func<HttpRequestMessage> requestCreator = () =>
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
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
                    var messages = JObject.Parse(responseString)["value"].ToObject<Message[]>();

                    return Json(messages, JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// Displays contact information.
        /// Minimal permission required: permission to read users' contacts.
        /// </summary>
        public async Task<ActionResult> Contacts()
        {
            // Obtain information for communicating with the service:
            Office365ServiceInfo serviceInfo = Office365ServiceInfo.GetExchangeServiceInfo();
            if (!serviceInfo.HasValidAccessToken)
            {
                return Redirect(serviceInfo.GetAuthorizationUrl(Request.Url));
            }

            // Create a URL for retrieving the data:
            string[] queryParameters = 
            {
                "$orderby=DisplayName",
                "$select=" + "DisplayName,EmailAddresses,BusinessPhones"
            };
            string requestUrl = String.Format(CultureInfo.InvariantCulture,
                "{0}/Me/Contacts?{1}",
                serviceInfo.ApiEndpoint,
                String.Join("&", queryParameters));

            // Prepare the HTTP request:
            using (HttpClient client = new HttpClient())
            {
                Func<HttpRequestMessage> requestCreator = () =>
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
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
                    var contacts = JObject.Parse(responseString)["value"].ToObject<Contact[]>();

                    return Json(contacts, JsonRequestBehavior.AllowGet);
                }
            }
        }


        [HttpPost]
        public async Task<ActionResult> RegisterCustomer(string customerName, string customerPhone, string customerEMail)
        {
            // Obtain information for communicating with the service:
            Office365ServiceInfo serviceInfo = Office365ServiceInfo.GetExchangeServiceInfo();
            if (!serviceInfo.HasValidAccessToken)
            {
                return Redirect(serviceInfo.GetAuthorizationUrl(Request.Url));
            }

            string requestUrl = String.Format(CultureInfo.InvariantCulture,
                "{0}/Me/Contacts",
                serviceInfo.ApiEndpoint
            );

            // Prepare the HTTP request:
            using (HttpClient client = new HttpClient())
            {
                Func<HttpRequestMessage> requestCreator = () =>
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                    request.Headers.Add("Accept", "application/json");
                    var contact = new Contact
                    {
                        DisplayName = customerName
                        ,
                        GivenName = customerName
                        ,
                        EmailAddresses = new EMailAddress[] { new EMailAddress { Address = customerEMail } }
                        ,
                        BusinessPhones = new string[] { customerPhone }
                    };
                    request.Content = new StringContent(JsonConvert.SerializeObject(contact));
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
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
                    var contacts = JObject.Parse(responseString).ToObject<Contact>();

                    return Json(contacts, JsonRequestBehavior.AllowGet);
                }
            }
        }


        [HttpPost]
        public async Task<ActionResult> ScheduleProduction(string customerName, string modelReferenceName, DateTime scheduledProductionDate)
        {
            // Obtain information for communicating with the service:
            Office365ServiceInfo serviceInfo = Office365ServiceInfo.GetExchangeServiceInfo();
            if (!serviceInfo.HasValidAccessToken)
            {
                return Redirect(serviceInfo.GetAuthorizationUrl(Request.Url));
            }

            string requestUrl = String.Format(CultureInfo.InvariantCulture,
                "{0}/Me/Events",
                serviceInfo.ApiEndpoint
            );

            // Prepare the HTTP request:
            using (HttpClient client = new HttpClient())
            {
                Func<HttpRequestMessage> requestCreator = () =>
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                    request.Headers.Add("Accept", "application/json");
                    var calendarEvent = new CalendarEvent
                    {
                        Start = scheduledProductionDate
                        ,
                        End = scheduledProductionDate
                        ,
                        Subject = modelReferenceName
                    };
                    // http://stackoverflow.com/questions/26399352/cant-create-new-events-with-office-365-api
                    var msg = @"
{
  'Subject': '" + customerName + @"',
  'Body': {
    'ContentType': 'HTML',
    'Content': '" + modelReferenceName + @"'
  },
  'Start': '" + scheduledProductionDate.AddHours(8).ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss").Replace(".", ":") + @"Z',
  'End': '" + scheduledProductionDate.AddHours(9).ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss").Replace(".", ":") + @"Z',
  'Location': {
      'DisplayName': 'LAB1'
    },
  'ShowAs': 'Busy'
}

";
                    request.Content = new StringContent(msg);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
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
                    var contacts = JObject.Parse(responseString).ToObject<CalendarEvent>();

                    return Json(contacts, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}
