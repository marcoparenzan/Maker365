using Maker365.Customers365.Contracts;
using Microsoft.AspNet.SignalR;
using System.Web.Mvc;

namespace Maker365.Utec365.Web.Controllers
{
    public class UtecCustomerOrdersQueriesController : Controller
    {
        private IHubContext _notify;
        private IUtecCustomerOrderQueries _utecCustomerOrderQueries;

        public UtecCustomerOrdersQueriesController(IUtecCustomerOrderQueries utecCustomerOrderQueries, IHubContext notify)
        {
            _notify = notify;
            _utecCustomerOrderQueries = utecCustomerOrderQueries;
        }

        public ActionResult Page(int pageNumber, int pageSize)
        {
            return Content(_utecCustomerOrderQueries.Page(0, 25).ToString(), "application/json");
        }
    }
}