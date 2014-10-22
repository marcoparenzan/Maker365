using Maker365.Customers365.Contracts;
using Microsoft.AspNet.SignalR;
using System.Web.Mvc;

namespace Maker365.Portal365.Web.Controllers
{
    public class CustomerOrdersQueriesController : Controller
    {
        private IHubContext _notify;
        private ICustomerOrderQueries _customerOrderQueries;

        public CustomerOrdersQueriesController(ICustomerOrderQueries customerOrderQueries, 
            IHubContext notify)
        {
            _notify = notify;
            _customerOrderQueries = customerOrderQueries;
        }

        public ActionResult Page(int pageNumber, int pageSize)
        {
            return Content(_customerOrderQueries.Page(0, 25).ToString(), "application/json");
        }
    }
}